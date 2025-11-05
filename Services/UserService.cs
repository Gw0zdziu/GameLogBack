using GameLogBack.DbContext;
using GameLogBack.Dtos;
using GameLogBack.Dtos.User;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace GameLogBack.Services;

public class UserService : IUserService
{
    private readonly GameLogDbContext _context;
    private readonly IEmailSenderHelper _emailSenderHelper;
    private readonly IPasswordHasher<UserLogins> _passwordHasher;
    private readonly IUtilsService _utilsService;

    public UserService(GameLogDbContext context, IPasswordHasher<UserLogins> passwordHasher, IUtilsService utilsService,
        IEmailSenderHelper emailSenderHelper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _utilsService = utilsService;
        _emailSenderHelper = emailSenderHelper;
    }

    public async Task<string> RegisterUser(RegisterNewUserDto registerNewUser)
    {
        var isUserNameExist = await _context.UserLogins.AnyAsync(x => x.UserName.ToLower() == registerNewUser.Username.ToLower());
        if (isUserNameExist) throw new BadRequestException("User with this username already exist");
        var isUserEmailExist = await _context.Users.AnyAsync(x => x.UserEmail.ToLower() == registerNewUser.UserEmail.ToLower());
        if (isUserEmailExist) throw new BadRequestException("User with this email already exist");
        var newUserId = Guid.NewGuid().ToString();
        var code = _utilsService.GenerateCodeToConfirmEmail();
        var newUser = new Users
        {
            UserId = newUserId,
            FirstName = registerNewUser.FirstName,
            LastName = registerNewUser.LastName,
            UserEmail = registerNewUser.UserEmail,
            UserLogins = new UserLogins
            {
                UserId = newUserId,
                UserName = registerNewUser.Username,
                Password = registerNewUser.Password
            },
            CodeConfirm = new CodeConfirmUsers
            {
                CodeId = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                UserId = newUserId,
                Code = code
            }
        };
        var passwordHash = _passwordHasher.HashPassword(newUser.UserLogins, registerNewUser.Password);
        newUser.UserLogins.Password = passwordHash;
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        await _emailSenderHelper.SendEmail(registerNewUser.UserEmail, "Kod potwierdzający użytkownika",
            $"Twój kod potwierdzający to : {code}");
        return newUserId;
    }

    public async Task<GetUserDto> GetUser(string userId)
    {
        var user = await _context.UserLogins.Join(_context.Users, userLogins => userLogins.UserId, users => users.UserId,
            (userLogins, users) => new GetUserDto
            {
                UserId = users.UserId,
                UserName = userLogins.UserName,
                UserEmail = users.UserEmail,
                IsActive = users.IsActive,
                FirstName = users.FirstName,
                LastName = users.LastName
            }).FirstOrDefaultAsync(x => x.UserId == userId);
        if (user is null) throw new BadRequestException("User not found");
        return user;
    }

    public async Task ResendNewConfirmCode(string userId)
    {
        var user = await _context.CodeConfirmUsers.FirstOrDefaultAsync(x => x.UserId == userId);
        if (user is null) throw new BadRequestException("User not found");
        var userEmail = _context.Users.Where(x => x.UserId == userId).Select(x => x.UserEmail).FirstOrDefault();
        var code = _utilsService.GenerateCodeToConfirmEmail();
        user.Code = code;
        user.ExpiryDate = DateTime.UtcNow.AddMinutes(15);
        await _context.SaveChangesAsync();
        await _emailSenderHelper.SendEmail(userEmail, "Kod potwierdzający użytkownika",
            $"Twój kod potwierdzający to : {code}");
    }

    public async Task ConfirmUser(ConfirmCodeDto confirmCodeDto)
    {
        var confirmCodeUser = await _context.CodeConfirmUsers.FirstOrDefaultAsync(x => x.UserId == confirmCodeDto.UserId);
        if (confirmCodeUser is null) throw new BadRequestException("Confirm code not found");
        if (confirmCodeUser.ExpiryDate < DateTime.UtcNow)
            throw new BadRequestException("Confirm code is expired. You must generate new code");
        if (confirmCodeUser.Code != confirmCodeDto.ConfirmCode)
            throw new BadRequestException("Confirm code is incorrect");
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == confirmCodeDto.UserId);
        if (user is null) throw new BadRequestException("User not found");
        user.IsActive = true;
        await _context.SaveChangesAsync();
    }

    public async Task RecoverPassword(string userEmail)
    {
        var user  = await _context.Users.FirstOrDefaultAsync(x => x.UserEmail == userEmail);
        if (user is null) 
        {
            throw new BadRequestException("User not found");
        }
        var code = _utilsService.GenerateCodeToRecoverPassword();
        var link = _utilsService.GenerateLinkToRecoveryPassword(code,user.UserId);
        var recoveryCode = await _context.CodeRecoveryPasswords.FirstOrDefaultAsync(x => x.UserId == user.UserId);
        if (recoveryCode is not null)
        {
            recoveryCode.Code = code;
            recoveryCode.ExpiryDate = DateTime.UtcNow.AddMinutes(15);
            recoveryCode.IsUsed = false;
            await _context.SaveChangesAsync();
            await _emailSenderHelper.SendEmail(userEmail, "Recovery password", link);
        }
        else
        {
            var newRecoveryPasswordCode = new CodeRecoveryPassword()
            {
                CodeId = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                UserId = user.UserId,
                Code = code,
                IsUsed = false
            };
            _context.Add(newRecoveryPasswordCode);
            await _context.SaveChangesAsync();
            await _emailSenderHelper.SendEmail(userEmail, "Recovery password", link);
        }
    }

    public async Task RecoveryUpdatePassword(RecoveryUpdatePasswordDto recoveryUpdatePasswordDto)
    {
        if (recoveryUpdatePasswordDto.NewPassword != recoveryUpdatePasswordDto.ConfirmPassword)
        {
            throw new BadRequestException("Passwords are not equal");
        }

        var user = await _context.Users
            .Include(x => x.UserLogins)
            .Include(x => x.CodeRecoveryPassword)
            .FirstOrDefaultAsync(x => x.UserId == recoveryUpdatePasswordDto.UserId && x.CodeRecoveryPassword.Code == recoveryUpdatePasswordDto.Token);
        if (user is null) throw new BadRequestException("User not found");
        if (user.CodeRecoveryPassword.IsUsed) throw new BadRequestException("Recovery code is used");
        if (user.CodeRecoveryPassword.ExpiryDate < DateTime.UtcNow) throw new BadRequestException("Recovery code is expired");
        var newPassword = _passwordHasher.HashPassword(user.UserLogins, recoveryUpdatePasswordDto.NewPassword);
        user.UserLogins.Password = newPassword;
        user.CodeRecoveryPassword.IsUsed = true;
        await _context.SaveChangesAsync();
        await _emailSenderHelper.SendEmail(user.UserEmail, "Aktualizacja hasła",
            "Pomyślnie zaktualizowano hasło");
    }


    public async Task UpdateUser(UpdateUserDto updateUserDto, string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
        if (user is null) throw new BadRequestException("User not found");
        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.UserEmail = updateUserDto.UserEmail;
        await _context.SaveChangesAsync();
    }
}
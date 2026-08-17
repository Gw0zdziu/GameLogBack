using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Dtos.User;
using GameLogBack.Dtos.User.RequestDto;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserLoginsRepository _userLoginsRepository;
    private readonly ICodeConfirmUsersRepository _codeConfirmUsersRepository;
    private readonly ICodeRecoveryPasswordsRepository _codeRecoveryPasswordsRepository;
    private readonly IEmailSenderHelper _emailSenderHelper;
    private readonly IPasswordHasher<UserLogins> _passwordHasher;
    private readonly IUtilsService _utilsService;

    public UserService(IPasswordHasher<UserLogins> passwordHasher, IUtilsService utilsService,
        IEmailSenderHelper emailSenderHelper, IUserRepository userRepository, IUserLoginsRepository userLoginsRepository, ICodeConfirmUsersRepository codeConfirmUsersRepository, ICodeRecoveryPasswordsRepository codeRecoveryPasswordsRepository)
    {
        _passwordHasher = passwordHasher;
        _utilsService = utilsService;
        _emailSenderHelper = emailSenderHelper;
        _userRepository = userRepository;
        _userLoginsRepository = userLoginsRepository;
        _codeConfirmUsersRepository = codeConfirmUsersRepository;
        _codeRecoveryPasswordsRepository = codeRecoveryPasswordsRepository;
    }

    public async Task<string> RegisterUser(RegisterNewUserDto registerNewUser)
    {
        var isUserNameExist = await _userLoginsRepository.CheckIfUserExists(registerNewUser.Username);
        if (isUserNameExist) throw new BadRequestException("User with this username already exist");
        var isUserEmailExist = await _userRepository.CheckIfUserExist(registerNewUser.UserEmail);
        if (isUserEmailExist) throw new BadRequestException("User with this email already exist");
        var newUserId = Guid.NewGuid().ToString();
        var code = _utilsService.GenerateCodeToConfirmEmail();
        var newUser = new Users
        {
            UserId = newUserId,
            FirstName = registerNewUser.FirstName,
            LastName = registerNewUser.LastName,
            UserEmail = registerNewUser.UserEmail,
            IsActive = false,
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
        /*invitationCodes.IsUsed = true;*/
        await _userRepository.Create(newUser);
        await _emailSenderHelper.SendEmail(registerNewUser.UserEmail, "Kod potwierdzający użytkownika",
            $"Twój kod potwierdzający to : {code}");
        return newUserId;
    }


    public async Task<GetUserDto> GetUser(string userId)
    {
        var user = await _userLoginsRepository.GetByUserId(userId);
        if (user is null)
        {
            throw new NotFoundException("User not found");
        }
        return new GetUserDto()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            UserEmail = user.User.UserEmail,
            IsActive = user.User.IsActive,
            FirstName = user.User.FirstName,
            LastName = user.User.LastName
        };
    }

    public async Task ResendNewConfirmCode(string userId)
    {
        var codeConfirmUsers = await _codeConfirmUsersRepository.GetByUserId(userId);
        var code = _utilsService.GenerateCodeToConfirmEmail();
        if (codeConfirmUsers == null)
        {
            codeConfirmUsers = new CodeConfirmUsers
            {
                UserId = userId,
                CodeId = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                Code = code
            };
            await _codeConfirmUsersRepository.Create(codeConfirmUsers);
        }
        else
        {
            codeConfirmUsers.Code = code;
            codeConfirmUsers.ExpiryDate = DateTime.UtcNow.AddMinutes(15);
            await _codeConfirmUsersRepository.Update(codeConfirmUsers);
        }
        var user = await _userRepository.GetById(userId);
        await _emailSenderHelper.SendEmail(user.UserEmail, "Kod potwierdzający użytkownika",
            $"Twój kod potwierdzający to : {code}");
    }

    public async Task ConfirmUser(ConfirmCodeDto confirmCodeDto)
    {
        var confirmCodeUser = await _codeConfirmUsersRepository.GetByUserId(confirmCodeDto.UserId);
        if (confirmCodeUser is null) throw new NotFoundException("Confirm code not found");
        if (confirmCodeUser.ExpiryDate < DateTime.UtcNow)
            throw new BadRequestException("Confirm code is expired. You must generate new code");
        if (confirmCodeUser.Code != confirmCodeDto.ConfirmCode)
            throw new BadRequestException("Confirm code is incorrect");
        var user = await _userRepository.GetById(confirmCodeDto.UserId);
        if (user is null) throw new NotFoundException("User not found");
        user.IsActive = true;
        await _userRepository.Update(user);
    }

    public async Task RecoverPassword(string userEmail)
    {
        var user = await _userRepository.GetByEmail(userEmail);
        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        var code = _utilsService.GenerateCodeToRecoverPassword();
        var link = _utilsService.GenerateLinkToRecoveryPassword(code, user.UserId);
        var recoveryCode = await _codeRecoveryPasswordsRepository.GetByUserId(user.UserId);
        if (recoveryCode is null)
        {
            var newRecoveryPasswordCode = new CodeRecoveryPassword()
            {
                CodeId = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                UserId = user.UserId,
                Code = code,
                IsUsed = false
            };
            await _codeRecoveryPasswordsRepository.Create(newRecoveryPasswordCode);
        }
        else
        {
            recoveryCode.Code = code;
            recoveryCode.ExpiryDate = DateTime.UtcNow.AddMinutes(15);
            recoveryCode.IsUsed = false;
            await _codeRecoveryPasswordsRepository.Update(recoveryCode);
        }
        await _emailSenderHelper.SendEmail(userEmail, "Recovery password", link);
    }

    public async Task UpdatePassword(RecoveryUpdatePasswordDto recoveryUpdatePasswordDto)
    {
        if (recoveryUpdatePasswordDto.NewPassword != recoveryUpdatePasswordDto.ConfirmPassword)
        {
            throw new BadRequestException("Passwords are not equal");
        }

        var user = await _userRepository.GetUserWithUserLoginsAndCodeRecovery(recoveryUpdatePasswordDto.UserId,
            recoveryUpdatePasswordDto.Token);
        if (user is null) throw new NotFoundException("User not found");
        if (user.CodeRecoveryPassword.IsUsed) throw new BadRequestException("Recovery code is used");
        if (user.CodeRecoveryPassword.ExpiryDate < DateTime.UtcNow)
            throw new BadRequestException("Recovery code is expired");
        var newPassword = _passwordHasher.HashPassword(user.UserLogins, recoveryUpdatePasswordDto.NewPassword);
        user.UserLogins.Password = newPassword;
        user.CodeRecoveryPassword.IsUsed = true;
        await _userRepository.Update(user);
        await _emailSenderHelper.SendEmail(user.UserEmail, "Aktualizacja hasła",
            "Pomyślnie zaktualizowano hasło");
    }
    
    public async Task UpdateUser(UpdateUserDto updateUserDto, string userId)
    {
        var user = await _userRepository.GetById(userId);
        if (user is null) throw new NotFoundException("User not found");
        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.UserEmail = updateUserDto.UserEmail;
        await _userRepository.Update(user);
    }
}

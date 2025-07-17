using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;

namespace GameLogBack.Services;

public class UserService : IUserService
{
    private readonly GameLogDbContext _context;
    private readonly IPasswordHasher<UserLogins> _passwordHasher;
    private readonly IUtilsService _utilsService;
    private readonly IEmailSenderHelper _emailSenderHelper;
    
    public UserService(GameLogDbContext context, IPasswordHasher<UserLogins> passwordHasher, IUtilsService utilsService, IEmailSenderHelper emailSenderHelper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _utilsService = utilsService;
        _emailSenderHelper = emailSenderHelper;
    } 
    
    public string RegisterUser(RegisterNewUserDto registerNewUser)
    {
        var isUserNameExist = _context.UserLogins
            .Any(x => x.UserName.ToLower() == registerNewUser.Username.ToLower());
        if (isUserNameExist)
        {
            throw new BadRequestException("User with this username already exist");
        }
        var isUserEmailExist = _context.Users.Any(x => x.UserEmail.ToLower() == registerNewUser.UserEmail.ToLower());
        if (isUserEmailExist)
        {
            throw new BadRequestException("User with this email already exist");
        }
        var newUserId = Guid.NewGuid().ToString();
        var code = _utilsService.GenerateCodeToConfirmEmail();
        var newUser = new Users()
        {
            UserId = newUserId,
            FirstName = registerNewUser.FirstName,
            LastName = registerNewUser.LastName,
            UserEmail = registerNewUser.UserEmail,
            UserLogins = new UserLogins()
            {
                UserId = newUserId,
                UserName = registerNewUser.Username,
                Password = registerNewUser.Password,
            },
            ConfirmCode = new ConfirmCodeUsers()
            {
                ConfirmCodeId = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                UserId = newUserId,
                ConfirmCode = code
            }
        };
        var passwordHash = _passwordHasher.HashPassword(newUser.UserLogins, registerNewUser.Password);
        newUser.UserLogins.Password = passwordHash;
        _context.Users.Add(newUser);
        _context.SaveChanges();
        _emailSenderHelper.SendEmail(registerNewUser.UserEmail, "Kod potwierdzający użytkownika", $"Twój kod potwierdzający to : {code}");
        return newUserId;
    }

    public GetUserDto GetUser(string userId)
    {
        var user = _context.UserLogins.Join(_context.Users, userLogins => userLogins.UserId, users => users.UserId,
            (userLogins, users) => new GetUserDto()
            {
                UserId = users.UserId,
                UserName = userLogins.UserName,
                UserEmail = users.UserEmail,
                IsActive = users.IsActive,
                FirstName = users.FirstName,
                LastName = users.LastName
            }).FirstOrDefault(x => x.UserId == userId);
        if (user is null)
        {
            throw new BadRequestException("User not found");       
        }
        return user;       
    }

    public void ResendNewConfirmCode(string userId)
    {
        var user = _context.ConfirmCodeUsers.FirstOrDefault(x => x.UserId == userId);
        if (user is null)
        {
            throw new BadRequestException("User not found");       
        }
        var userEmail = _context.Users.Where(x => x.UserId == userId).Select(x => x.UserEmail).FirstOrDefault();
        var code = _utilsService.GenerateCodeToConfirmEmail();
        user.ConfirmCode = code;
        user.ExpiryDate = DateTime.UtcNow.AddMinutes(15);
        _context.SaveChanges();
        _emailSenderHelper.SendEmail(userEmail, "Kod potwierdzający użytkownika", $"Twój kod potwierdzający to : {code}");
    }

    public void ConfirmUser(ConfirmCodeDto confirmCodeDto)
    {
        var confirmCodeUser = _context.ConfirmCodeUsers.FirstOrDefault(x => x.UserId == confirmCodeDto.UserId);
        if (confirmCodeUser is null)
        {
            throw new BadRequestException("Confirm code not found");       
        }
        if (confirmCodeUser.ExpiryDate < DateTime.UtcNow)
        {
            throw new BadRequestException("Confirm code is expired. You must generate new code");  
        }
        if (confirmCodeUser.ConfirmCode != confirmCodeDto.ConfirmCode)
        {
            throw new BadRequestException("Confirm code is incorrect");
        }
        var user = _context.Users.FirstOrDefault(x => x.UserId == confirmCodeDto.UserId);
        if (user is null)
        {
            throw new BadRequestException("User not found");       
        }
        user.IsActive = true;
        _context.SaveChanges();
    }
    

    public void UpdateUser(UpdateUserDto updateUserDto, string userId)
    {
        var user = _context.Users.FirstOrDefault(x => x.UserId == userId);
        if (user is null)
        {
            throw new BadRequestException("User not found");
        }
        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.UserEmail = updateUserDto.UserEmail;
        _context.SaveChanges();
    }
}
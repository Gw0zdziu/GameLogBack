using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameLogBack.Authentication;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameLogBack.Services;

public class UserService : IUserService
{
    private readonly GameLogDbContext _context;
    private readonly IPasswordHasher<UserLogins> _passwordHasher;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IUtilsService _utilsService;
    private readonly IEmailSenderHelper _emailSenderHelper;
    
    public UserService(GameLogDbContext context, IPasswordHasher<UserLogins> passwordHasher, AuthenticationSettings authenticationSettings, IUtilsService utilsService, IEmailSenderHelper emailSenderHelper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
        _utilsService = utilsService;
        _emailSenderHelper = emailSenderHelper;
    } 
    
    public void RegisterUser(RegisterNewUserDto registerNewUser)
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
    }

    public LoginResponseDto LoginUser(LoginUserDto loginUserDto)
    {
        var user = _context.UserLogins.FirstOrDefault(x => x.UserName == loginUserDto.UserName);
        if (user is null)
        {
            throw new BadHttpRequestException("Data of login is incorrect");
        }
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginUserDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Data of login is incorrect");
        }

        var token = _utilsService.GetToken(user);
        var refreshToken = _utilsService.GetRefreshToken();
        var refreshTokenInfo = _context.RefreshTokens.FirstOrDefault(x => x.UserId == user.UserId);
        if (refreshTokenInfo is null)
        {
            _context.RefreshTokens.Add(new RefreshTokenInfo()
            {
                UserId = user.UserId,
                RefreshTokenId = Guid.NewGuid().ToString(),
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(_authenticationSettings.JwtAccessTokenExpireDays)
            });
            _context.SaveChanges();
        }
        var loginResponse = new LoginResponseDto()
        {
            Token = token,
            RefreshToken = refreshToken,
        };
        return loginResponse;
    }

    public TokenInfoDto GetRefreshToken(TokenInfoDto tokenInfo)
    {
            var principal = _utilsService.GetPrincipalFromExpiredToken(tokenInfo.AccessToken);
            var userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var refreshTokenInfo = _context.RefreshTokens.FirstOrDefault(x => x.UserId == userId);
            if (refreshTokenInfo is null || refreshTokenInfo.RefreshToken != tokenInfo.RefreshToken || refreshTokenInfo.ExpiryDate < DateTime.Now)
            {
                throw new BadRequestException("Refresh token is expired");
            }

            var user = _context.UserLogins.FirstOrDefault(x => x.UserId == userId);
            var token = _utilsService.GetToken(user);
            var newRefreshToken = _utilsService.GetRefreshToken();
            refreshTokenInfo.RefreshToken = newRefreshToken;
            _context.SaveChanges();
            var tokenInfoDto = new TokenInfoDto()
            {
                AccessToken = token,
                RefreshToken = newRefreshToken
            };
            return tokenInfoDto;
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
        var user = _context.ConfirmCodeUsers.FirstOrDefault(x => x.UserId == confirmCodeDto.UserId);
        if (user is null)
        {
            throw new BadRequestException("User not found");       
        }
        if (user.ExpiryDate < DateTime.UtcNow)
        {
            throw new BadRequestException("Confirm code is expired. You must generate new code");  
        }
        if (user.ConfirmCode != confirmCodeDto.ConfirmCode)
        {
            throw new BadRequestException("Confirm code is incorrect");
        }
        user.ConfirmCode = null;
        _context.SaveChanges();
    }

    public void LogoutUser(string userId)
    {
        var refreshTokenInfo = _context.RefreshTokens.FirstOrDefault(x => x.UserId == userId);
        if (refreshTokenInfo is null)
        {
            throw new BadRequestException("Refresh token is expired");
        }
        _context.RefreshTokens.Remove(refreshTokenInfo);
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
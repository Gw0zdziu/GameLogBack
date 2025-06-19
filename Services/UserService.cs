using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameLogBack.Authentication;
using GameLogBack.DbContext;
using GameLogBack.Entities;
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
    
    public UserService(GameLogDbContext context, IPasswordHasher<UserLogins> passwordHasher, AuthenticationSettings authenticationSettings)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
    } 
    
    public void RegisterUser(RegisterNewUserDto registerNewUser)
    {
        var isUserNameExist = _context.UserLogins
            .Any(x => x.UserName.ToLower() == registerNewUser.Username.ToLower());
        if (isUserNameExist)
        {
            throw new BadHttpRequestException("User already exist");
        }
        var newUserId = Guid.NewGuid().ToString();
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
            }
        };
        var passwordHash = _passwordHasher.HashPassword(newUser.UserLogins, registerNewUser.Password);
        newUser.UserLogins.Password = passwordHash;
        _context.Users.Add(newUser);
        _context.SaveChanges(); 
    }

    public string LoginUser(LoginUserDto loginUserDto)
    {
        var user = _context.UserLogins.FirstOrDefault(x => x.UserName == loginUserDto.UserName);
        if (user is null)
        {
            throw new BadHttpRequestException("Data of login is incorrect");
        }
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginUserDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadHttpRequestException("Data of login is incorrect");
        }

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);
        var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer, _authenticationSettings.JwtIssuer, claims, expires, signingCredentials: credentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}
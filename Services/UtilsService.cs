using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GameLogBack.Authentication;
using GameLogBack.Entities;
using GameLogBack.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace GameLogBack.Services;

public class UtilsService : IUtilsService
{
    private readonly AuthenticationSettings _authenticationSettings;

    public UtilsService(AuthenticationSettings authenticationSettings)
    {
        _authenticationSettings = authenticationSettings;
    }

    public string GetRefreshToken()
    {
        var refreshToken = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(refreshToken);
        return Convert.ToBase64String(refreshToken);
    }

    public string GetToken(UserLogins userLogins)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userLogins.UserName),
            new Claim(ClaimTypes.NameIdentifier, userLogins.UserId),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);
        var jwtToken = new JwtSecurityToken(_authenticationSettings.JwtIssuer, _authenticationSettings.JwtIssuer, claims, expires, signingCredentials: credentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(jwtToken);
        return token;
    }
}
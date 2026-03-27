using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GameLogBack.Authentication;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameLogBack.Services;

public class UtilsService : IUtilsService
{
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IConfiguration _configuration;

    public UtilsService(AuthenticationSettings authenticationSettings, IConfiguration configuration)
    {
        _authenticationSettings = authenticationSettings;
        _configuration = configuration;
    }

    public string GetRefreshToken()
    {
        var refreshToken = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(refreshToken);
        return Convert.ToBase64String(refreshToken);
    }

    public string GetToken(UserLogins userLogins, int expireIn)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userLogins.UserName),
            new(ClaimTypes.NameIdentifier, userLogins.UserId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresTime = DateTime.UtcNow.AddMinutes(expireIn);
        var jwtToken = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
            _authenticationSettings.JwtIssuer, claims, null, expiresTime,
            credentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(jwtToken);
        return token;
    }

    public string GetAccessToken(UserLogins userLogins, string refreshToken)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userLogins.UserName),
            new(ClaimTypes.NameIdentifier, userLogins.UserId)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshToken));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(_authenticationSettings.JwtAccessTokenExpireDays);
        var jwtAccessToken = new JwtSecurityToken(_authenticationSettings.JwtIssuer, _authenticationSettings.JwtIssuer,
            claims, expires, signingCredentials: credentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(jwtAccessToken);
        return accessToken;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = _authenticationSettings.JwtIssuer,
            ValidIssuer = _authenticationSettings.JwtIssuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey)),
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new BadRequestException("Invalid token");

        return principal;
    }

    public string GenerateCodeToConfirmEmail()
    {
        var randomNumber = new Random();
        return randomNumber.Next(0, 9999).ToString("D4");
    }

    public string GenerateCodeToRecoverPassword()
    {
        var randomByte = new byte[8];
        var randomNumber = RandomNumberGenerator.Create();
        randomNumber.GetBytes(randomByte);
        var code = Convert.ToBase64String(randomByte);
        return code;
    }

    public string GenerateLinkToRecoveryPassword(string recoverCode, string userId)
    {
        var frontedUrl = _configuration.GetSection("FrontendUrl").Value;
        var recoveryPasswordEndpoint = _configuration.GetSection("RecoveryPasswordEndpoint").Value;
        recoveryPasswordEndpoint = recoveryPasswordEndpoint?.Replace("{userId}", userId).Replace("{token", recoverCode);
        var linkWithCode = frontedUrl + recoveryPasswordEndpoint;
        return linkWithCode;
    }

    public async Task<PaginatedResults<T>> GetPaginatedData<T>(IQueryable<T> data, PaginatedQuery paginatedQuery)
    {
        var totalAmount = await data.CountAsync();
        var paginatedList = await data
            .Skip((paginatedQuery.PageNumber - 1) * paginatedQuery.PageSize)
            .Take(paginatedQuery.PageSize).ToListAsync();
        var firstItemIndexList = (paginatedQuery.PageNumber - 1) * paginatedList.Count() + 1;
        var lastItemIndexList = firstItemIndexList + paginatedList.Count() - 1;
        var amountPages = (int)Math.Ceiling((double)totalAmount / paginatedQuery.PageSize);
        var amountPagesList = Enumerable.Range(1, amountPages).ToList();
        var paginatedResult = new PaginatedResults<T>
        {
            Results = paginatedList,
            TotalAmount = totalAmount,
            PageNumber = paginatedQuery.PageNumber,
            PageSize = paginatedQuery.PageSize,
            FirstItemIndexList = firstItemIndexList,
            LastItemIndexList = lastItemIndexList,
            AmountPagesList = amountPagesList
        };
        return paginatedResult;
    }
}
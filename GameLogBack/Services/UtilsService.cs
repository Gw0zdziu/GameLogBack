using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using GameLogBack.Constants;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GameLogBack.Services;

public class UtilsService : IUtilsService
{
    private readonly IOptions<AuthenticationSettings> _authenticationSettings;
    private readonly IOptions<FrontendSettings> _frontendSettings;

    public UtilsService( IOptions<FrontendSettings> frontendSettings, IOptions<AuthenticationSettings> authenticationSettings)
    {
        _frontendSettings = frontendSettings;
        _authenticationSettings = authenticationSettings;
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
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.Value.JwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresTime = DateTime.UtcNow.AddMinutes(expireIn);
        var jwtToken = new JwtSecurityToken(_authenticationSettings.Value.JwtIssuer,
            _authenticationSettings.Value.JwtIssuer, claims, null, expiresTime,
            credentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(jwtToken);
        return token;
    }



    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = _authenticationSettings.Value.JwtIssuer,
            ValidIssuer = _authenticationSettings.Value.JwtIssuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.Value.JwtKey)),
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        ClaimsPrincipal principal;
        try
        {
            principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        }
        catch (ArgumentNullException _)
        {
            throw new BadRequestException("Token is null", ErrorCodes.Utils.UtilsTokenIsNull);
        }
        catch (SecurityTokenMalformedException _)
        {
            throw new BadRequestException("Token has invalid format", ErrorCodes.Utils.UtilsTokenHasInvalidFormat);
        }
        catch (SecurityTokenExpiredException _)
        {
            throw new BadRequestException("Token is expired", ErrorCodes.Utils.UtilsTokenExpired);
        }
        catch (Exception _)
        {
            throw new BadRequestException("Token is invalid", ErrorCodes.Utils.UtilsTokenIsInvalid);
        }

        return principal;
    }

    public string GenerateCodeToConfirmEmail()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
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
        var recoveryLink = new StringBuilder(_frontendSettings.Value.FrontendUrl)
            .Append(_frontendSettings.Value.RecoveryPasswordEndpoint)
            .Replace("{userId}", userId)
            .Replace("{token}", recoverCode)
            .ToString();
        return recoveryLink;
    }
}

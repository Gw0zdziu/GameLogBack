using System.Security.Claims;
using GameLogBack.Authentication;
using GameLogBack.DbContext;
using GameLogBack.Dtos.Auth;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Services;

public class AuthService : IAuthService
{
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly GameLogDbContext _context;
    private readonly IPasswordHasher<UserLogins> _passwordHasher;
    private readonly IUtilsService _utilsService;


    public AuthService(GameLogDbContext context, AuthenticationSettings authenticationSettings,
        IPasswordHasher<UserLogins> passwordHasher, IUtilsService utilsService)
    {
        _context = context;
        _authenticationSettings = authenticationSettings;
        _passwordHasher = passwordHasher;
        _utilsService = utilsService;
    }

    public async Task<LoginResponseDto> LoginUser(LoginUserDto loginUserDto)
    {
        var user = await _context.UserLogins.FirstOrDefaultAsync(x => x.UserName == loginUserDto.UserName);
        if (user is null) throw new BadRequestException("Data of login is incorrect");
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginUserDto.Password);
        if (result == PasswordVerificationResult.Failed) throw new BadRequestException("Data of login is incorrect");

        var token = _utilsService.GetToken(user);
        var refreshToken = _utilsService.GetRefreshToken();
        var refreshTokenInfo = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == user.UserId);
        if (refreshTokenInfo is null)
        {
            _context.RefreshTokens.Add(new RefreshTokenInfo
            {
                UserId = user.UserId,
                RefreshTokenId = Guid.NewGuid().ToString(),
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(_authenticationSettings.JwtAccessTokenExpireDays)
            });
        }
        else
        {
            refreshTokenInfo.RefreshToken = refreshToken;
            refreshTokenInfo.ExpiryDate = DateTime.UtcNow.AddDays(_authenticationSettings.JwtAccessTokenExpireDays);
        }

        await _context.SaveChangesAsync();
        var loginResponse = new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            UserId = user.UserId
        };
        return loginResponse;
    }

    public async Task<TokenInfoDto> GetRefreshToken(TokenInfoDto tokenInfo)
    {
        var principal = _utilsService.GetPrincipalFromExpiredToken(tokenInfo.AccessToken);
        var userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        var refreshTokenInfo = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId);
        if (refreshTokenInfo is null || refreshTokenInfo.ExpiryDate < DateTime.UtcNow)
        {
            throw new BadRequestException("Refresh token is expired");
        }
        var user = _context.UserLogins.FirstOrDefault(x => x.UserId == userId);
        var token = _utilsService.GetToken(user);
        var newRefreshToken = _utilsService.GetRefreshToken();
        refreshTokenInfo.RefreshToken = newRefreshToken;
        await _context.SaveChangesAsync();
        var tokenInfoDto = new TokenInfoDto
        {
            AccessToken = token,
            RefreshToken = newRefreshToken
        };
        return tokenInfoDto;
    }


    public async Task LogoutUser(string userId)
    {
        var refreshTokenInfo = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId);
        if (refreshTokenInfo is null) throw new BadRequestException("");
        _context.RefreshTokens.Remove(refreshTokenInfo);
        await _context.SaveChangesAsync();
    }
}
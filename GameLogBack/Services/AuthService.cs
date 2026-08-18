using System.Security.Claims;
using GameLogBack.DataAccess.Interfaces;
using GameLogBack.Dtos.Auth;
using GameLogBack.Dtos.Auth.RequestDto;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Services;

public class AuthService : IAuthService
{
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IUserLoginsRepository _userLoginsRepository;
    private readonly IRefreshTokenInfoRepository _refreshTokenInfoRepository;
    private readonly IPasswordHasher<UserLogins> _passwordHasher;
    private readonly IUtilsService _utilsService;


    public AuthService(AuthenticationSettings authenticationSettings,
        IPasswordHasher<UserLogins> passwordHasher, IUtilsService utilsService, IUserLoginsRepository userLoginsRepository, IRefreshTokenInfoRepository refreshTokenInfoRepository)
    {
        _authenticationSettings = authenticationSettings;
        _passwordHasher = passwordHasher;
        _utilsService = utilsService;
        _userLoginsRepository = userLoginsRepository;
        _refreshTokenInfoRepository = refreshTokenInfoRepository;
    }

    public async Task<string> LoginUser(LoginUserDto loginUserDto)
    {
        var user = await _userLoginsRepository.GetByUserName(loginUserDto.UserName);
        if (user is null) throw new BadRequestException("Data of login is incorrect");
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginUserDto.Password);
        if (result == PasswordVerificationResult.Failed) throw new BadRequestException("Data of login is incorrect");
        var token = _utilsService.GetToken(user, _authenticationSettings.JwtTokenExpireMinutes);
        var refreshToken = _utilsService.GetRefreshToken();
        var refreshTokenInfo = await _refreshTokenInfoRepository.GetByUserId(user.UserId);
        if (refreshTokenInfo is null)
        {
            var newRefreshTokenInfo = new RefreshTokenInfo
            {
                UserId = user.UserId,
                RefreshTokenId = Guid.NewGuid().ToString(),
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtAccessTokenExpireMinutes)
            };
            await _refreshTokenInfoRepository.Create(newRefreshTokenInfo);
        }
        else
        {
            refreshTokenInfo.ExpiryDate = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtAccessTokenExpireMinutes);
            refreshTokenInfo.RefreshToken = refreshToken;
            await _refreshTokenInfoRepository.Update(refreshTokenInfo);
        }
        
        return token;
    }

    public async Task<string> GetRefreshToken(string tokenInfo)
    {
        var principal = _utilsService.GetPrincipalFromExpiredToken(tokenInfo);
        var userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        var refreshTokenInfo = await _refreshTokenInfoRepository.GetByUserId(userId);
        if (refreshTokenInfo is null || refreshTokenInfo.ExpiryDate < DateTime.UtcNow)
            throw new BadRequestException("Refresh token is expired");
        var user = await _userLoginsRepository.GetByUserId(userId);
        var token = _utilsService.GetToken(user, _authenticationSettings.JwtAccessTokenExpireMinutes);
        return token;
    }


    public async Task LogoutUser(string userId)
    {
        var refreshTokenInfo = await _refreshTokenInfoRepository.GetByUserId(userId);
        if (refreshTokenInfo is null) throw new BadRequestException("Refresh token is expired");
        await _refreshTokenInfoRepository.Delete(refreshTokenInfo);
    }
}

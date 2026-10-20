using System.Security.Claims;
using GameLogBack.Constants;
using GameLogBack.DataAccess.Interfaces;
using GameLogBack.Dtos.Auth.RequestDto;
using GameLogBack.Dtos.Auth.ResponseDto;
using GameLogBack.Dtos.User;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Helpers;
using GameLogBack.Interfaces;
using GameLogBack.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GameLogBack.Services;

public class AuthService : IAuthService
{
    private readonly IOptions<AuthenticationSettings> _authenticationSettings;
    private readonly IUserLoginsRepository _userLoginsRepository;
    private readonly IRefreshTokenInfoRepository _refreshTokenInfoRepository;
    private readonly IPasswordHasher<UserLogins> _passwordHasher;
    private readonly IUtilsService _utilsService;


    public AuthService(
        IPasswordHasher<UserLogins> passwordHasher, IUtilsService utilsService, IUserLoginsRepository userLoginsRepository, IRefreshTokenInfoRepository refreshTokenInfoRepository, IOptions<AuthenticationSettings> authenticationSettings)
    {
        _passwordHasher = passwordHasher;
        _utilsService = utilsService;
        _userLoginsRepository = userLoginsRepository;
        _refreshTokenInfoRepository = refreshTokenInfoRepository;
        _authenticationSettings = authenticationSettings;
    }

    public async Task<LoginResponseDto> LoginUser(LoginUserDto loginUserDto)
    {
        var user = await _userLoginsRepository.GetByUserName(loginUserDto.UserName);
        if (user is null) throw new UnauthorizedException("Data of login is incorrect", ErrorCodes.Auth.IncorrectDataOfLogin);
        if (!user.User.IsActive) throw new UnauthorizedException("Inactive account", ErrorCodes.Auth.InactiveAccount);
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginUserDto.Password);
        if (result == PasswordVerificationResult.Failed) throw new UnauthorizedException("Data of login is incorrect", ErrorCodes.Auth.IncorrectDataOfLogin);
        var token = _utilsService.GetToken(user, _authenticationSettings.Value.JwtAccessTokenExpireMinutes);
        var refreshToken = _utilsService.GetRefreshToken();
        var hashedRefreshToken = HashHelper.GetHashValue(refreshToken);
        var refreshTokenInfo = await _refreshTokenInfoRepository.GetByUserId(user.UserId);
        if (refreshTokenInfo is null)
        {
            var newRefreshTokenInfo = new RefreshTokenInfo
            {
                UserId = user.UserId,
                RefreshToken = hashedRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_authenticationSettings.Value.JwtRefreshTokenExpireMinutes)
            };
            await _refreshTokenInfoRepository.Create(newRefreshTokenInfo);
        }
        else
        {
            refreshTokenInfo.RefreshToken = hashedRefreshToken;
            refreshTokenInfo.ExpiryDate =
                DateTime.UtcNow.AddMinutes(_authenticationSettings.Value.JwtRefreshTokenExpireMinutes);
            await _refreshTokenInfoRepository.Update(refreshTokenInfo);
        }
        return new LoginResponseDto()
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresIn = _authenticationSettings.Value.JwtAccessTokenExpireMinutes,
            User = new GetUserDto()
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FirstName = user.User.FirstName,
                LastName = user.User.LastName,
                UserEmail = user.User.UserEmail,
                IsActive = user.User.IsActive
            }
        };
    }

    public async Task<string> GetRefreshToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedException("Incorrect token", ErrorCodes.Auth.ExpiredRefreshToken);
        }
        var hashedRefreshToken = HashHelper.GetHashValue(token);
        var refreshTokenInfo = await _refreshTokenInfoRepository.GetByRefreshToken(hashedRefreshToken);
        if (refreshTokenInfo is null || refreshTokenInfo.ExpiryDate < DateTime.UtcNow)
            throw new UnauthorizedException("Incorrect token", ErrorCodes.Auth.ExpiredRefreshToken);
        var user = await _userLoginsRepository.GetByUserId(refreshTokenInfo.UserId);
        if (user is null || !user.User.IsActive)
        {
            throw new UnauthorizedException("Incorrect token", ErrorCodes.Auth.ExpiredRefreshToken);
        }
        var newToken = _utilsService.GetToken(user, _authenticationSettings.Value.JwtAccessTokenExpireMinutes);
        return newToken;
    }


    public async Task LogoutUser(string userId)
    {
        var refreshTokenInfo = await _refreshTokenInfoRepository.GetByUserId(userId);
        if (refreshTokenInfo is null) return;
        await _refreshTokenInfoRepository.Delete(refreshTokenInfo);
    }
}



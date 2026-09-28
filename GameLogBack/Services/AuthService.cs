using System.Security.Claims;
using GameLogBack.Constants;
using GameLogBack.DataAccess.Interfaces;
using GameLogBack.Dtos.Auth.RequestDto;
using GameLogBack.Dtos.Auth.ResponseDto;
using GameLogBack.Dtos.User;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Settings;
using Microsoft.AspNetCore.Identity;

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

    public async Task<LoginResponseDto> LoginUser(LoginUserDto loginUserDto)
    {
        var user = await _userLoginsRepository.GetByUserName(loginUserDto.UserName);
        if (user is null) throw new BadRequestException("Data of login is incorrect", ErrorCodes.Auth.IncorrectDataOfLogin);
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginUserDto.Password);
        if (result == PasswordVerificationResult.Failed) throw new BadRequestException("Data of login is incorrect", ErrorCodes.Auth.IncorrectDataOfLogin);
        var token = _utilsService.GetToken(user, _authenticationSettings.JwtAccessTokenExpireMinutes);
        var refreshToken = _utilsService.GetRefreshToken();
        var refreshTokenInfo = await _refreshTokenInfoRepository.GetByUserId(user.UserId);
        if (refreshTokenInfo is null)
        {
            var newRefreshTokenInfo = new RefreshTokenInfo
            {
                UserId = user.UserId,
                RefreshTokenId = Guid.NewGuid().ToString(),
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtRefreshTokenExpireMinutes)
            };
            await _refreshTokenInfoRepository.Create(newRefreshTokenInfo);
        }
        else
        {
            refreshTokenInfo.ExpiryDate = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtRefreshTokenExpireMinutes);
            refreshTokenInfo.RefreshToken = refreshToken;
            await _refreshTokenInfoRepository.Update(refreshTokenInfo);
        }
        
        return new LoginResponseDto()
        {
            Token = token,
            ExpiresIn = _authenticationSettings.JwtAccessTokenExpireMinutes,
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

    public async Task<string> GetRefreshToken(string tokenInfo)
    {
        var principal = _utilsService.GetPrincipalFromExpiredToken(tokenInfo);
        var userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        var refreshTokenInfo = await _refreshTokenInfoRepository.GetByUserId(userId);
        if (refreshTokenInfo is null || refreshTokenInfo.ExpiryDate < DateTime.UtcNow)
            throw new BadRequestException("Refresh token is expired", ErrorCodes.Auth.ExpiredRefreshToken);
        var user = await _userLoginsRepository.GetByUserId(userId);
        var token = _utilsService.GetToken(user, _authenticationSettings.JwtRefreshTokenExpireMinutes);
        return token;
    }


    public async Task LogoutUser(string userId)
    {
        var refreshTokenInfo = await _refreshTokenInfoRepository.GetByUserId(userId);
        if (refreshTokenInfo is null) throw new BadRequestException("Token is expired. Please log in again", ErrorCodes.Auth.ExpiredAccessToken);
        await _refreshTokenInfoRepository.Delete(refreshTokenInfo);
    }
}



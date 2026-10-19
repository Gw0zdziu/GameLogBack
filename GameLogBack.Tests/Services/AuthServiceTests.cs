using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GameLogBack.DataAccess.Interfaces;
using GameLogBack.Dtos.Auth.RequestDto;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Services;
using GameLogBack.Settings;
using GameLogBack.Tests.Helpers;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace GameLogBack.Tests.Services;

[TestSubject(typeof(AuthService))]
public class AuthServiceTests
{
    private readonly Mock<IUserLoginsRepository> _mockUserLoginsRepository = new();
    private readonly Mock<IRefreshTokenInfoRepository>_mockRefreshTokenInfoRepository = new();
    private readonly Mock<IPasswordHasher<UserLogins>> _mockPasswordHasher = new();
    private readonly Mock<IUtilsService> _mockUtilsService = new();
    private readonly Mock<IOptions<AuthenticationSettings>> _mockAuthenticationSettings = new();

    public AuthServiceTests()
    {
        var mockAuthenticationData = new AuthenticationSettings()
        {
            JwtAccessTokenExpireMinutes = 15,
            JwtRefreshTokenExpireMinutes = 21600,
            JwtIssuer = "jwtIssuer",
            JwtKey = "key"
        };
        _mockAuthenticationSettings.Setup(x => x.Value).Returns(mockAuthenticationData);
    }

    #region LoginUser

    [Fact]
    public async Task LoginUser_ForValidDataAndRefreshTokenExist_UpdateRefreshTokenAndReturnsToken()
    {
        
        //Arrange
        var loginUserDto = new LoginUserDto()
        {
            UserName = "nickName",
            Password = "password"
        };
        var userLogins = new UserLogins()
        {
            UserId = "1",
            UserName = "nickName",
            Password = "password",
            User = new Users()
            {
                UserId = "1",
                UserEmail = "email@gmail.com",
                IsActive = true,
                FirstName = "firstName",
                LastName = "lastName",
            }
            
        };
        var refreshTokenInfo = new RefreshTokenInfo()
        {
            RefreshTokenId = "1",
            UserId = "1",
            RefreshToken = "oldRefreshToken",
            ExpiryDate = new DateTime(2026, 01, 01)
        };
        _mockUserLoginsRepository.Setup(x => x.GetByUserName(It.IsAny<string>()))
            .ReturnsAsync(userLogins);
        _mockPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(It.IsAny<UserLogins>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        _mockUtilsService.Setup(x => x.GetToken(It.IsAny<UserLogins>(), It.IsAny<int>())).Returns("newToken");
        _mockUtilsService.Setup(x => x.GetRefreshToken()).Returns("refreshToken");
        _mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(refreshTokenInfo);
        
        //Act
        var service = new AuthService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockUserLoginsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockAuthenticationSettings.Object);
        var result =  await service.LoginUser(loginUserDto);
        
        //Assert
        _mockRefreshTokenInfoRepository.Verify(x => x.Update(It.IsAny<RefreshTokenInfo>()), Times.Once);
        result.Token.Should().Be("newToken");

    }
    
    [Fact]
    public async Task LoginUser_ForValidDataAndRefreshTokenNotExist_CreateRefreshTokenAndReturnsToken()
    {
        //Arrange
        var loginUserDto = new LoginUserDto()
        {
            UserName = "nickName",
            Password = "password",
        };
        var userLogins = new UserLogins()
        {
            UserId = "1",
            UserName = "nickName",
            Password = "password",
            User = new Users()
            {
                UserId = "1",
                UserEmail = "email@gmail.com",
                IsActive = true,
                FirstName = "firstName",
                LastName = "lastName",
            }
        };
        _mockUserLoginsRepository.Setup(x => x.GetByUserName(It.IsAny<string>()))
            .ReturnsAsync(userLogins);
        _mockPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(It.IsAny<UserLogins>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        _mockUtilsService.Setup(x => x.GetToken(It.IsAny<UserLogins>(), It.IsAny<int>())).Returns("newToken");
        _mockUtilsService.Setup(x => x.GetRefreshToken()).Returns("refreshToken");
        _mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((RefreshTokenInfo)null);
        
        //Act
        var service = new AuthService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockUserLoginsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockAuthenticationSettings.Object);
        var result =  await service.LoginUser(loginUserDto);
        
        //Assert
        _mockRefreshTokenInfoRepository.Verify(x => x.Create(It.IsAny<RefreshTokenInfo>()), Times.Once);
        result.Token.Should().Be("newToken");
    }
    
    [Fact]
    public async Task LoginUser_UserLoginsNotExist_ThrowsBadRequestException()
    {
        
        //Arrange
        var loginUserDto = new LoginUserDto()
        {
            UserName = "nickName",
            Password = "password"
        };
        _mockUserLoginsRepository.Setup(x => x.GetByUserName(It.IsAny<string>()))
            .ReturnsAsync((UserLogins)null);
        _mockPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(It.IsAny<UserLogins>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        _mockUtilsService.Setup(x => x.GetToken(It.IsAny<UserLogins>(), It.IsAny<int>())).Returns("newToken");
        _mockUtilsService.Setup(x => x.GetRefreshToken()).Returns("refreshToken");
        _mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((RefreshTokenInfo)null);
        
        //Act
        var service = new AuthService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockUserLoginsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockAuthenticationSettings.Object);
        var result =  async () => await service.LoginUser(loginUserDto);
        
        //Assert
        await result.Should().ThrowAsync<UnauthorizedException>().WithMessage("Data of login is incorrect");
    }
    
    [Fact]
    public async Task LoginUser_PasswordNotVerified_ThrowsBadRequestException()
    {
        
        //Arrange
        var loginUserDto = new LoginUserDto()
        {
            UserName = "nickName",
            Password = "password"
        };
        var userLogins = new UserLogins()
        {
            UserId = "1",
            UserName = "nickName",
            Password = "password",
            User = new Users()
            {
                IsActive = true
            }
        };
        _mockUserLoginsRepository.Setup(x => x.GetByUserName(It.IsAny<string>()))
            .ReturnsAsync(userLogins);
        _mockPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(It.IsAny<UserLogins>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Failed);
        _mockUtilsService.Setup(x => x.GetToken(It.IsAny<UserLogins>(), It.IsAny<int>())).Returns("newToken");
        _mockUtilsService.Setup(x => x.GetRefreshToken()).Returns("refreshToken");
        _mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((RefreshTokenInfo)null);
        
        //Act
        var service = new AuthService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockUserLoginsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockAuthenticationSettings.Object);
        var result =  async () => await service.LoginUser(loginUserDto);
        
        //Assert
        await result.Should().ThrowAsync<UnauthorizedException>().WithMessage("Data of login is incorrect");
    }
    
    [Fact]
    public async Task LoginUser_PasswordNotVerified_ThrowsBadRequestExceptionUserNotActivated()
    {
        
        //Arrange
        var loginUserDto = new LoginUserDto()
        {
            UserName = "nickName",
            Password = "password"
        };
        var userLogins = new UserLogins()
        {
            UserId = "1",
            UserName = "nickName",
            Password = "password",
            User = new Users()
            {
                IsActive = false
            }
        };
        _mockUserLoginsRepository.Setup(x => x.GetByUserName(It.IsAny<string>()))
            .ReturnsAsync(userLogins);
        _mockPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(It.IsAny<UserLogins>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Failed);
        _mockUtilsService.Setup(x => x.GetToken(It.IsAny<UserLogins>(), It.IsAny<int>())).Returns("newToken");
        _mockUtilsService.Setup(x => x.GetRefreshToken()).Returns("refreshToken");
        _mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((RefreshTokenInfo)null);
        
        //Act
        var service = new AuthService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockUserLoginsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockAuthenticationSettings.Object);
        var result =  async () => await service.LoginUser(loginUserDto);
        
        //Assert
        await result.Should().ThrowAsync<UnauthorizedException>().WithMessage("Inactive account");
    }

    #endregion

    #region GetRefreshToken

    [Fact]
    public async Task GetRefreshToken_ForValidData_ReturnNewRefreshToken()
    {
        //Arrange
        var refreshTokenInfo = new RefreshTokenInfo()
        {
            RefreshTokenId = "1",
            UserId = "1",
            RefreshToken =  Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("oldRefreshToken"))),
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };
        var userLogins = new UserLogins()
        {
            UserId = "1",
            UserName = "nickName",
            Password = "password",
            User = new Users()
            {
                IsActive = true
            }
        };
        var mockClaimsPrincipal = ClaimsPrincipalTestHelper.CreatePrincipal("1", "nickName", "admin");
        _mockUtilsService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(mockClaimsPrincipal);
        _mockRefreshTokenInfoRepository.Setup(x => x.GetByRefreshToken(It.IsAny<string>())).ReturnsAsync(refreshTokenInfo);
        _mockUserLoginsRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(userLogins);
        _mockUtilsService.Setup(x => x.GetToken(It.IsAny<UserLogins>(), It.IsAny<int>())).Returns("newToken");
        
        //Act
        var service = new AuthService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockUserLoginsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockAuthenticationSettings.Object);
        var result = await service.GetRefreshToken("oldRefreshToken");
        
        //Assert
        result.Should().Be("newToken");
    }
    
    [Fact]
    public async Task GetRefreshToken_RefreshTokenIsNull_ThrowsBadRequestException()
    {
        //Arrange
        var mockClaimsPrincipal = ClaimsPrincipalTestHelper.CreatePrincipal("1", "nickName", "admin");
        _mockUtilsService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(mockClaimsPrincipal);
        _mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((RefreshTokenInfo)null);
        
        //Act
        var service = new AuthService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockUserLoginsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockAuthenticationSettings.Object);
        var result = async () => await service.GetRefreshToken("oldRefreshToken");
        
        //Assert
        await result.Should().ThrowAsync<UnauthorizedException>().WithMessage("Incorrect token");

    }
    
    [Fact]
    public async Task GetRefreshToken_RefreshTokenIsExpired_ThrowsBadRequestException()
    {
        //Arrange
        var mockClaimsPrincipal = ClaimsPrincipalTestHelper.CreatePrincipal("1", "nickName", "admin");
        var refreshTokenInfo = new RefreshTokenInfo()
        {
            RefreshTokenId = "1",
            UserId = "1",
            RefreshToken = "oldRefreshToken",
            ExpiryDate = new DateTime(2026, 07, 01)
        };
        _mockUtilsService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(mockClaimsPrincipal);
        _mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(refreshTokenInfo);
        
        //Act
        var service = new AuthService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockUserLoginsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockAuthenticationSettings.Object);
        var result = async () => await service.GetRefreshToken("oldRefreshToken");
        
        //Assert
        await result.Should().ThrowAsync<UnauthorizedException>().WithMessage("Incorrect token");
    }

    #endregion

    #region LogoutUser

    [Fact]
    public async Task LogoutUser_ForValidData_ShouldCallDeleteMethod()
    {
        //Arrange
        var refreshTokenInfo = new RefreshTokenInfo()
        {
            RefreshTokenId = "1",
            UserId = "1",
            RefreshToken = "oldRefreshToken",
            ExpiryDate = new DateTime(2026, 10, 01)
        };
        _mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(refreshTokenInfo);
        
        //Act
        var service = new AuthService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockUserLoginsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockAuthenticationSettings.Object);
        await service.LogoutUser("1");
        
        //Assert
        _mockRefreshTokenInfoRepository.Verify(x => x.Delete(It.IsAny<RefreshTokenInfo>()), Times.Once);
    }


    #endregion
    
    
}
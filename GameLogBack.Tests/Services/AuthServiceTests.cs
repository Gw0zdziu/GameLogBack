using System;
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
using Moq;
using Xunit;

namespace GameLogBack.Tests.Services;

[TestSubject(typeof(AuthService))]
public class AuthServiceTests
{
    [Fact]
    public async Task LoginUser_ForValidDataAndRefreshTokenExist_UpdateRefreshTokenAndReturnsToken()
    {
        
        //Arrange
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockRefreshTokenInfoRepository = new Mock<IRefreshTokenInfoRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var mockAuthenticationSettings = new AuthenticationSettings()
        {
            JwtKey = "jwtKey",
            JwtAccessTokenExpireMinutes = 15,
            JwtTokenExpireMinutes = 21600,
            JwtIssuer = "https://auth.example.com"
        };
        var loginUserDto = new LoginUserDto()
        {
            UserName = "nickName",
            Password = "password"
        };
        var userLogins = new UserLogins()
        {
            UserId = "1",
            UserName = "nickName",
            Password = "password"
        };
        var refreshTokenInfo = new RefreshTokenInfo()
        {
            RefreshTokenId = "1",
            UserId = "1",
            RefreshToken = "oldRefreshToken",
            ExpiryDate = new DateTime(2026, 01, 01)
        };
        mockUserLoginsRepository.Setup(x => x.GetByUserName(It.IsAny<string>()))
            .ReturnsAsync(userLogins);
        mockPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(It.IsAny<UserLogins>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        mockUtilsService.Setup(x => x.GetToken(It.IsAny<UserLogins>(), It.IsAny<int>())).Returns("newToken");
        mockUtilsService.Setup(x => x.GetRefreshToken()).Returns("refreshToken");
        mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(refreshTokenInfo);
        
        //Act
        var service = new AuthService(mockAuthenticationSettings, mockPasswordHasher.Object, mockUtilsService.Object, mockUserLoginsRepository.Object, mockRefreshTokenInfoRepository.Object);
        var result =  await service.LoginUser(loginUserDto);
        
        //Assert
        mockRefreshTokenInfoRepository.Verify(x => x.Update(It.IsAny<RefreshTokenInfo>()), Times.Once);
        result.Should().Be("newToken");

    }
    
    [Fact]
    public async Task LoginUser_ForValidDataAndRefreshTokenNotExist_CreateRefreshTokenAndReturnsToken()
    {
        
        //Arrange
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockRefreshTokenInfoRepository = new Mock<IRefreshTokenInfoRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var mockAuthenticationSettings = new AuthenticationSettings()
        {
            JwtKey = "jwtKey",
            JwtAccessTokenExpireMinutes = 15,
            JwtTokenExpireMinutes = 21600,
            JwtIssuer = "https://auth.example.com"
        };
        var loginUserDto = new LoginUserDto()
        {
            UserName = "nickName",
            Password = "password"
        };
        var userLogins = new UserLogins()
        {
            UserId = "1",
            UserName = "nickName",
            Password = "password"
        };
        mockUserLoginsRepository.Setup(x => x.GetByUserName(It.IsAny<string>()))
            .ReturnsAsync(userLogins);
        mockPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(It.IsAny<UserLogins>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        mockUtilsService.Setup(x => x.GetToken(It.IsAny<UserLogins>(), It.IsAny<int>())).Returns("newToken");
        mockUtilsService.Setup(x => x.GetRefreshToken()).Returns("refreshToken");
        mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((RefreshTokenInfo)null);
        
        //Act
        var service = new AuthService(mockAuthenticationSettings, mockPasswordHasher.Object, mockUtilsService.Object, mockUserLoginsRepository.Object, mockRefreshTokenInfoRepository.Object);
        var result =  await service.LoginUser(loginUserDto);
        
        //Assert
        mockRefreshTokenInfoRepository.Verify(x => x.Create(It.IsAny<RefreshTokenInfo>()), Times.Once);
        result.Should().Be("newToken");
    }
    
    [Fact]
    public async Task LoginUser_UserLoginsNotExist_ThrowsBadRequestException()
    {
        
        //Arrange
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockRefreshTokenInfoRepository = new Mock<IRefreshTokenInfoRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var mockAuthenticationSettings = new AuthenticationSettings()
        {
            JwtKey = "jwtKey",
            JwtAccessTokenExpireMinutes = 15,
            JwtTokenExpireMinutes = 21600,
            JwtIssuer = "https://auth.example.com"
        };
        var loginUserDto = new LoginUserDto()
        {
            UserName = "nickName",
            Password = "password"
        };
        mockUserLoginsRepository.Setup(x => x.GetByUserName(It.IsAny<string>()))
            .ReturnsAsync((UserLogins)null);
        mockPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(It.IsAny<UserLogins>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Success);
        mockUtilsService.Setup(x => x.GetToken(It.IsAny<UserLogins>(), It.IsAny<int>())).Returns("newToken");
        mockUtilsService.Setup(x => x.GetRefreshToken()).Returns("refreshToken");
        mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((RefreshTokenInfo)null);
        
        //Act
        var service = new AuthService(mockAuthenticationSettings, mockPasswordHasher.Object, mockUtilsService.Object, mockUserLoginsRepository.Object, mockRefreshTokenInfoRepository.Object);
        var result =  async () => await service.LoginUser(loginUserDto);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Data of login is incorrect");
    }
    
    [Fact]
    public async Task LoginUser_PasswodNotVerified_ThrowsBadRequestException()
    {
        
        //Arrange
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockRefreshTokenInfoRepository = new Mock<IRefreshTokenInfoRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var mockAuthenticationSettings = new AuthenticationSettings()
        {
            JwtKey = "jwtKey",
            JwtAccessTokenExpireMinutes = 15,
            JwtTokenExpireMinutes = 21600,
            JwtIssuer = "https://auth.example.com"
        };
        var loginUserDto = new LoginUserDto()
        {
            UserName = "nickName",
            Password = "password"
        };
        var userLogins = new UserLogins()
        {
            UserId = "1",
            UserName = "nickName",
            Password = "password"
        };
        mockUserLoginsRepository.Setup(x => x.GetByUserName(It.IsAny<string>()))
            .ReturnsAsync(userLogins);
        mockPasswordHasher.Setup(x =>
                x.VerifyHashedPassword(It.IsAny<UserLogins>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(PasswordVerificationResult.Failed);
        mockUtilsService.Setup(x => x.GetToken(It.IsAny<UserLogins>(), It.IsAny<int>())).Returns("newToken");
        mockUtilsService.Setup(x => x.GetRefreshToken()).Returns("refreshToken");
        mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((RefreshTokenInfo)null);
        
        //Act
        var service = new AuthService(mockAuthenticationSettings, mockPasswordHasher.Object, mockUtilsService.Object, mockUserLoginsRepository.Object, mockRefreshTokenInfoRepository.Object);
        var result =  async () => await service.LoginUser(loginUserDto);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Data of login is incorrect");
    }

    [Fact]
    public async Task GetRefreshToken_ForValidData_ReturnNewRefreshToken()
    {
        //Arrange
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockRefreshTokenInfoRepository = new Mock<IRefreshTokenInfoRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var mockAuthenticationSettings = new AuthenticationSettings()
        {
            JwtKey = "jwtKey",
            JwtAccessTokenExpireMinutes = 15,
            JwtTokenExpireMinutes = 21600,
            JwtIssuer = "https://auth.example.com"
        };
        var refreshTokenInfo = new RefreshTokenInfo()
        {
            RefreshTokenId = "1",
            UserId = "1",
            RefreshToken = "oldRefreshToken",
            ExpiryDate = new DateTime(2026, 10, 01)
        };
        var userLogins = new UserLogins()
        {
            UserId = "1",
            UserName = "nickName",
            Password = "password"
        };
        var mockClaimsPrincipal = ClaimsPrincipalTestHelper.CreatePrincipal("1", "nickName", "admin");
        mockUtilsService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(mockClaimsPrincipal);
        mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(refreshTokenInfo);
        mockUserLoginsRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(userLogins);
        mockUtilsService.Setup(x => x.GetToken(It.IsAny<UserLogins>(), It.IsAny<int>())).Returns("newToken");
        
        //Act
        var service = new AuthService(mockAuthenticationSettings, mockPasswordHasher.Object, mockUtilsService.Object, mockUserLoginsRepository.Object, mockRefreshTokenInfoRepository.Object);
        var result = await service.GetRefreshToken("oldRefreshToken");
        
        //Assert
        result.Should().Be("newToken");
    }
    
    [Fact]
    public async Task GetRefreshToken_RefreshTokenIsNull_ThrowsBadRequestException()
    {
        //Arrange
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockRefreshTokenInfoRepository = new Mock<IRefreshTokenInfoRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var mockAuthenticationSettings = new AuthenticationSettings()
        {
            JwtKey = "jwtKey",
            JwtAccessTokenExpireMinutes = 15,
            JwtTokenExpireMinutes = 21600,
            JwtIssuer = "https://auth.example.com"
        };
        var mockClaimsPrincipal = ClaimsPrincipalTestHelper.CreatePrincipal("1", "nickName", "admin");
        mockUtilsService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(mockClaimsPrincipal);
        mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((RefreshTokenInfo)null);
        
        //Act
        var service = new AuthService(mockAuthenticationSettings, mockPasswordHasher.Object, mockUtilsService.Object, mockUserLoginsRepository.Object, mockRefreshTokenInfoRepository.Object);
        var result = async () => await service.GetRefreshToken("oldRefreshToken");
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Refresh token is expired");
    }
    
    [Fact]
    public async Task GetRefreshToken_RefreshTokenIsExpired_ThrowsBadRequestException()
    {
        //Arrange
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockRefreshTokenInfoRepository = new Mock<IRefreshTokenInfoRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var mockClaimsPrincipal = ClaimsPrincipalTestHelper.CreatePrincipal("1", "nickName", "admin");
        var mockAuthenticationSettings = new AuthenticationSettings()
        {
            JwtKey = "jwtKey",
            JwtAccessTokenExpireMinutes = 15,
            JwtTokenExpireMinutes = 21600,
            JwtIssuer = "https://auth.example.com"
        };
        var refreshTokenInfo = new RefreshTokenInfo()
        {
            RefreshTokenId = "1",
            UserId = "1",
            RefreshToken = "oldRefreshToken",
            ExpiryDate = new DateTime(2026, 07, 01)
        };
        mockUtilsService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(mockClaimsPrincipal);
        mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(refreshTokenInfo);
        
        //Act
        var service = new AuthService(mockAuthenticationSettings, mockPasswordHasher.Object, mockUtilsService.Object, mockUserLoginsRepository.Object, mockRefreshTokenInfoRepository.Object);
        var result = async () => await service.GetRefreshToken("oldRefreshToken");
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Refresh token is expired");
    }

    [Fact]
    public async Task LogoutUser_ForValidData_ShouldCallDeleteMethod()
    {
        //Arrange
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockRefreshTokenInfoRepository = new Mock<IRefreshTokenInfoRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var mockAuthenticationSettings = new AuthenticationSettings()
        {
            JwtKey = "jwtKey",
            JwtAccessTokenExpireMinutes = 15,
            JwtTokenExpireMinutes = 21600,
            JwtIssuer = "https://auth.example.com"
        };
        var refreshTokenInfo = new RefreshTokenInfo()
        {
            RefreshTokenId = "1",
            UserId = "1",
            RefreshToken = "oldRefreshToken",
            ExpiryDate = new DateTime(2026, 10, 01)
        };
        mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(refreshTokenInfo);
        
        //Act
        var service = new AuthService(mockAuthenticationSettings, mockPasswordHasher.Object, mockUtilsService.Object, mockUserLoginsRepository.Object, mockRefreshTokenInfoRepository.Object);
        await service.LogoutUser("1");
        
        //Assert
        mockRefreshTokenInfoRepository.Verify(x => x.Delete(It.IsAny<RefreshTokenInfo>()), Times.Once);
    }
    
    [Fact]
    public async Task LogoutUser_InvalidRefreshToken_ThrowsBadRequestException()
    {
        //Arrange
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockRefreshTokenInfoRepository = new Mock<IRefreshTokenInfoRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var mockAuthenticationSettings = new AuthenticationSettings()
        {
            JwtKey = "jwtKey",
            JwtAccessTokenExpireMinutes = 15,
            JwtTokenExpireMinutes = 21600,
            JwtIssuer = "https://auth.example.com"
        };
        mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((RefreshTokenInfo)null);
        
        //Act
        var service = new AuthService(mockAuthenticationSettings, mockPasswordHasher.Object, mockUtilsService.Object, mockUserLoginsRepository.Object, mockRefreshTokenInfoRepository.Object);
        var result = async () => await service.LogoutUser("1");
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Refresh token is expired");
        
        
    }
}
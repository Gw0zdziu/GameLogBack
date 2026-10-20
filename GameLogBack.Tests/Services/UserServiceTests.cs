using System;
using System.Threading.Tasks;
using FluentAssertions;
using GameLogBack.DataAccess.Interfaces;
using GameLogBack.Dtos.User;
using GameLogBack.Dtos.User.RequestDto;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace GameLogBack.Tests.Services;

[TestSubject(typeof(UserService))]
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IUserLoginsRepository> _mockUserLoginsRepository = new();
    private readonly Mock<ICodeConfirmUsersRepository> _mockCodeConfirmUsersRepository = new();
    private readonly Mock<ICodeRecoveryPasswordsRepository> _mockCodeRecoveryPasswordsRepository = new();
    private readonly Mock<IRefreshTokenInfoRepository> _mockRefreshTokenInfoRepository = new();
    private readonly Mock<IInvitationCodesRepository> _mockInvitationCodesRepository = new();
    private readonly Mock<IEmailSenderHelper> _mockEmailSenderHelper = new();
    private readonly Mock<IPasswordHasher<UserLogins>> _mockPasswordHasher = new();
    private readonly Mock<IUtilsService> _mockUtilsService = new();
    
    
    #region RegisterUser
    
    [Fact]
    public async Task RegisterUser_ForValidData_ReturnsUserId()
    {
        //Arrange
        var registerNewUser = new RegisterNewUserDto()
        {
            UserEmail = "email@gmail.com",
            Username = "nickName",
            Password = "PlainPassword",
            ConfirmPassword = "PlainPassword",
            FirstName = "Piotr",
            LastName = "Nowak",
            InvitationCode = "1234"
        };
        var invitationCode = new InvitationCodes()
        {
            InvitationCode = "12456",
            IsUsed = false
        };
        _mockInvitationCodesRepository.Setup(x => x.GetActiveInvitationCode(It.IsAny<string>()))
            .ReturnsAsync(invitationCode);
        _mockUserLoginsRepository.Setup(x => x.CheckIfUserExists(It.IsAny<string>())).ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.CheckIfExistsUserByEmail(It.IsAny<string>())).ReturnsAsync(false);
        _mockUtilsService.Setup(x => x.GenerateCodeToConfirmEmail()).Returns("1234");
        _mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<UserLogins>(), It.IsAny<string>())).Returns("hashedPassword");
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);
        var result = await userService.RegisterUser(registerNewUser);
        
        //Assert
        _mockUserRepository.Verify(x => x.Create(It.IsAny<Users>()), Times.Once);
        _mockEmailSenderHelper.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        result.Should().NotBeNullOrEmpty();

    }
    
    [Fact]
    public async Task RegisterUser_UserNameExist_ThrowBadRequestException()
    {
        //Arrange
        var registerNewUser = new RegisterNewUserDto()
        {
            UserEmail = "email@gmail.com",
            Username = "nickName",
            Password = "PlainPassword",
            ConfirmPassword = "PlainPassword",
            FirstName = "Piotr",
            LastName = "Nowak",
            InvitationCode = "1234"
        };
        _mockUserLoginsRepository.Setup(x => x.CheckIfUserExists(It.IsAny<string>())).ReturnsAsync(true);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);       
        var result = async () => await userService.RegisterUser(registerNewUser);
        
        //Assert
        await result.Should().ThrowAsync<ConflictException>().WithMessage("Username is already taken");
    }
    
    [Fact]
    public async Task RegisterUser_UserEmail_ThrowBadRequestException()
    {
        //Arrange
        
        var registerNewUser = new RegisterNewUserDto()
        {
            UserEmail = "email@gmail.com",
            Username = "nickName",
            Password = "PlainPassword",
            ConfirmPassword = "PlainPassword",
            FirstName = "Piotr",
            LastName = "Nowak",
            InvitationCode = "1234"
        };
        _mockUserLoginsRepository.Setup(x => x.CheckIfUserExists(It.IsAny<string>())).ReturnsAsync(false);
        _mockUserRepository.Setup(x => x.CheckIfExistsUserByEmail(It.IsAny<string>())).ReturnsAsync(true);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);        
        var result = async () => await userService.RegisterUser(registerNewUser);
        
        //Assert
        await result.Should().ThrowAsync<ConflictException>().WithMessage("Email is already taken");

    }
    
    #endregion
    
    #region GetUser
    
    [Fact]
    public async Task GetUser_ForValidParameters_ReturnUserData()
    {
        //Arrange
        var userLogins = new UserLogins()
        {
            UserId = "1",
            UserName = "JoeDeer",
            Password = "Password",
            User = new Users()
            {
                UserEmail = "email@gmail.com",
                IsActive = true,
                FirstName = "Piotr",
                LastName = "Nowak"
            }
        };
        _mockUserLoginsRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(userLogins);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);            
        var result = await userService.GetUser("1");
        
        //Assert
        result.UserName.Should().Be("JoeDeer");
    }

    [Fact]
    public async Task GetUser_ForInvalidUserId_ThrowNotFoundException()
    {
        //Arrange
        _mockUserLoginsRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((UserLogins)null);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);
        var result = async () => await userService.GetUser("1");
        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
    }
    
    #endregion

    #region ResendNewConfirmCode

    [Fact]
    public async Task ResendNewConfirmCode_ForValidParameters_ShouldCallUpdateMethod()
    {
        //Arrange
        var codeConfirmUsers = new CodeConfirmUsers()
        {
            Code = "1234",
            UserId = "1",
            ExpiryDate = new DateTime(2026, 02, 01)
        };
        var user = new Users()
        {
            UserEmail = "email@gmail.com"
        };
        _mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(codeConfirmUsers);
        _mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object); 
        await userService.ResendNewConfirmCode("1");
        
        //Assert
        _mockCodeConfirmUsersRepository.Verify(x => x.Update(It.IsAny<CodeConfirmUsers>()), Times.Once);
    }

    [Fact]
    public async Task ResendNewConfirmCode_ForInvalidParameters_ShouldCallCreateMethod()
    {
        //Arrange
        var user = new Users()
        {
            UserEmail = "email@gmail.com"
        };
        _mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((CodeConfirmUsers)null);
        _mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);   
        await userService.ResendNewConfirmCode("1");
        
        //Assert
        _mockCodeConfirmUsersRepository.Verify(x => x.Create(It.IsAny<CodeConfirmUsers>()), Times.Once);
    }

    #endregion

    #region ConfirmUser

    [Fact]
    public async Task ConfirmUser_ForValidParameters_ShouldUpdateUser()
    {
        //Arrange
        var confirmCodeDto = new ConfirmCodeDto()
        {
            UserId = "1",
            ConfirmCode = "1234"
        };
        var confirmCode = new CodeConfirmUsers()
        {
            Code = "1234",
            UserId = "1",
            ExpiryDate = DateTime.Now.AddDays(1)
        };
        var user = new Users()
        {
            IsActive = false
        };
        _mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync(confirmCode);
        _mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);  
        await userService.ConfirmUser(confirmCodeDto);
        
        //Assert
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Users>()), Times.Once);
        user.IsActive.Should().BeTrue();
    }
    
    [Fact]
    public async Task ConfirmUser_ForNullableCodeConfirmUsers_NotFoundException()
    {
        //Arrange
        var confirmCodeDto = new ConfirmCodeDto()
        {
            UserId = "1",
            ConfirmCode = "1234"
        };
        _mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync((CodeConfirmUsers)null);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);
        var result = async () => await userService.ConfirmUser(confirmCodeDto);
        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("Verification code not found");
    }
    
    [Fact]
    public async Task ConfirmUser_ExpiryDateIsOlderCurrentTime_BadRequestException()
    {
        //Arrange
        var confirmCode = new CodeConfirmUsers()
        {
            Code = "1234",
            UserId = "1",
            ExpiryDate = DateTime.Now.AddDays(-1)
        };
        var confirmCodeDto = new ConfirmCodeDto()
        {
            UserId = "1",
            ConfirmCode = "1234"
        };
        _mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync(confirmCode);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);  
        var result = async () => await userService.ConfirmUser(confirmCodeDto);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Verification code has expired");
    }
    
    [Fact]
    public async Task ConfirmUser_CodesAreDifferent_BadRequestException()
    {
        //Arrange
        var confirmCode = new CodeConfirmUsers()
        {
            Code = "12345",
            UserId = "1",
            ExpiryDate = DateTime.Now.AddDays(1)
        };
        var confirmCodeDto = new ConfirmCodeDto()
        {
            UserId = "1",
            ConfirmCode = "1234"
        };
        _mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync(confirmCode);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);  
        var result = async () => await userService.ConfirmUser(confirmCodeDto);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Verification code is incorrect");
    }
    
    [Fact]
    public async Task ConfirmUser_NullableUser_NotFoundException()
    {
        //Arrange
        var confirmCode = new CodeConfirmUsers()
        {
            Code = "1234",
            UserId = "1",
            ExpiryDate = DateTime.Now.AddDays(1)
        };
        var confirmCodeDto = new ConfirmCodeDto()
        {
            UserId = "1",
            ConfirmCode = "1234"
        };
        _mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync(confirmCode);
        _mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync((Users)null);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);  
        var result = async () => await userService.ConfirmUser(confirmCodeDto);
        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
    }


    #endregion
    
    #region RecoverPassword
    
    [Fact]
    public async Task RecoverPassword_IfRecoveryCodeExist_ShouldCallUpdateCodeMethod()
    {
        var user = new Users()
        {
            UserId = "1"
        };
        var codeRecoveryPassword = new CodeRecoveryPassword()
        {
            Code = "1234",
            UserId = "1",
            ExpiryDate = new DateTime(2026, 10, 01),
            IsUsed = false
        };
        _mockUserRepository.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync(user);
        _mockUtilsService.Setup(x => x.GenerateCodeToRecoverPassword()).Returns("1234");
        _mockUtilsService.Setup(x => x.GenerateLinkToRecoveryPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("http://localhost:5000/recovery-password/1234");
        _mockCodeRecoveryPasswordsRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync(codeRecoveryPassword);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);  
        await userService.RecoverPassword("email@gmail.com");
        
        //Assert
        _mockCodeRecoveryPasswordsRepository.Verify(x => x.Update(It.IsAny<CodeRecoveryPassword>()), Times.Once);
        _mockEmailSenderHelper.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

    }
    
    [Fact]
    public async Task RecoverPassword_IfRecoveryCodeNotExist_ShouldCallCreateCodeMethod()
    {
        var user = new Users()
        {
            UserId = "1"
        };
        _mockUserRepository.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync(user);
        _mockUtilsService.Setup(x => x.GenerateCodeToRecoverPassword()).Returns("1234");
        _mockUtilsService.Setup(x => x.GenerateLinkToRecoveryPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("http://localhost:5000/recovery-password/1234");
        _mockCodeRecoveryPasswordsRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync((CodeRecoveryPassword)null);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object); 
        await userService.RecoverPassword("email@gmail.com");
        
        //Assert
        _mockCodeRecoveryPasswordsRepository.Verify(x => x.Create(It.IsAny<CodeRecoveryPassword>()), Times.Once);
        _mockEmailSenderHelper.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task RecoverPassword_IfUserNotExist_ThrowBadRequestException()
    {
        _mockUserRepository.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync((Users)null);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object); 
        var result = async () => await userService.RecoverPassword("email@gmail.com");

        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
    }
    
    #endregion

    #region UpdatePassword

    [Fact]
    public async Task UpdatePassword_IfPasswordsAreCorrect_ShouldUpdateUserPassword()
    {
        var user = new Users()
        {
            UserEmail = "email@gmail.com",
            CodeRecoveryPassword = new CodeRecoveryPassword()
            {
                IsUsed = false,
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            },
            UserLogins = new UserLogins()
            {
                Password = "password"
            }
        };
        var recoveryUpdatePasswordDto = new RecoveryUpdatePasswordDto()
        {
            UserId = "1",
            NewPassword = "password",
            ConfirmPassword = "password",
            Token = "1234"
        };
        var refreshTokenInfo = new RefreshTokenInfo()
        {
            RefreshToken = "refreshToken",
            UserId = "1",
            ExpiryDate = DateTime.Now.AddDays(1),

        };
        _mockUserRepository.Setup(x => x.GetUserWithUserLoginsAndCodeRecovery(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);
        _mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<UserLogins>(), It.IsAny<string>())).Returns("hashedPassword");
        _mockRefreshTokenInfoRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(refreshTokenInfo);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);  
        await userService.UpdatePassword(recoveryUpdatePasswordDto);
        
        //Assert
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Users>()), Times.Once);
        _mockEmailSenderHelper.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockRefreshTokenInfoRepository.Verify(x => x.Delete(It.IsAny<RefreshTokenInfo>()), Times.Once);
        user.UserLogins.Password.Should().Be("hashedPassword");
    }
    
    [Fact]
    public async Task UpdatePassword_PasswordsIsDifferent_ThrowBadRequestException()
    {
        var user = new Users()
        {
            UserEmail = "email@gmail.com",
            CodeRecoveryPassword = new CodeRecoveryPassword()
            {
                IsUsed = false,
                ExpiryDate = new DateTime(2026, 10, 01)
            },
            UserLogins = new UserLogins()
            {
                Password = "password"
            }
        };
        var recoveryUpdatePasswordDto = new RecoveryUpdatePasswordDto()
        {
            UserId = "1",
            NewPassword = "password",
            ConfirmPassword = "confirmPassword",
            Token = "1234"
        };
        _mockUserRepository.Setup(x => x.GetUserWithUserLoginsAndCodeRecovery(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);
        _mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<UserLogins>(), It.IsAny<string>())).Returns("hashedPassword");
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object); 
        var result = async () => await userService.UpdatePassword(recoveryUpdatePasswordDto);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Passwords do not match");
    }
        
    [Fact]
    public async Task UpdatePassword_UserNotExist_ThrowBadRequestException()
    {
        var recoveryUpdatePasswordDto = new RecoveryUpdatePasswordDto()
        {
            UserId = "1",
            NewPassword = "password",
            ConfirmPassword = "password",
            Token = "1234"
        };
        _mockUserRepository.Setup(x => x.GetUserWithUserLoginsAndCodeRecovery(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((Users)null);
        _mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<UserLogins>(), It.IsAny<string>())).Returns("hashedPassword");
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);  
        var result = async () => await userService.UpdatePassword(recoveryUpdatePasswordDto);
        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
    }
    
    [Fact]
    public async Task UpdatePassword_DateIsExpired_ThrowBadRequestException()
    {
        
        var user = new Users()
        {
            UserEmail = "email@gmail.com",
            CodeRecoveryPassword = new CodeRecoveryPassword()
            {
                IsUsed = false,
                ExpiryDate = new DateTime(2026, 01, 01)
            },
            UserLogins = new UserLogins()
            {
                Password = "password"
            }
        };
        var recoveryUpdatePasswordDto = new RecoveryUpdatePasswordDto()
        {
            UserId = "1",
            NewPassword = "password",
            ConfirmPassword = "password",
            Token = "1234"
        };
        _mockUserRepository.Setup(x => x.GetUserWithUserLoginsAndCodeRecovery(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);
        _mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<UserLogins>(), It.IsAny<string>())).Returns("hashedPassword");
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);  
        var result = async () => await userService.UpdatePassword(recoveryUpdatePasswordDto);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Recovery code has expired");
    }

    #endregion

    #region UpdateUser

    [Fact]
    public async Task UpdateUser_IfUserExist_ShouldUpdateUser()
    {
        var user = new Users()
        {
            FirstName = "Piotr",
            LastName = "Nowak",
            UserEmail = "email@gmail.com"
        };
        var updateUserDto = new UpdateUserDto()
        {
            FirstName = "Jan",
            LastName = "Nowak",
            UserEmail = "email@gmail.com"
        };
        _mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);  
        await userService.UpdateUser(updateUserDto, "1");
        
        //Assert
        _mockUserRepository.Verify(x => x.Update(It.IsAny<Users>()), Times.Once);
        
    }

    [Fact]
    public async Task UpdateUser_IfUserNotExist_ThrowNotFoundException()
    {
        var updateUserDto = new UpdateUserDto()
        {
            FirstName = "Jan",
            LastName = "Nowak",
            UserEmail = "email@gmail.com"
        };
        _mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync((Users)null);
        
        //Act
        var userService = new UserService(_mockPasswordHasher.Object, _mockUtilsService.Object, _mockEmailSenderHelper.Object, 
            _mockUserRepository.Object, _mockUserLoginsRepository.Object, 
            _mockCodeConfirmUsersRepository.Object, _mockCodeRecoveryPasswordsRepository.Object, _mockRefreshTokenInfoRepository.Object, _mockInvitationCodesRepository.Object);  
        var result = async () => await userService.UpdateUser(updateUserDto, "1");
        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        
    }

    #endregion
    
}
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
    [Fact]
    public async Task RegisterUser_ForValidData_ReturnsUserId()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
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
        mockUserLoginsRepository.Setup(x => x.CheckIfUserExists(It.IsAny<string>())).ReturnsAsync(false);
        mockUserRepository.Setup(x => x.CheckIfUserExist(It.IsAny<string>())).ReturnsAsync(false);
        mockUtilsService.Setup(x => x.GenerateCodeToConfirmEmail()).Returns("1234");
        mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<UserLogins>(), It.IsAny<string>())).Returns("hashedPassword");
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = await userService.RegisterUser(registerNewUser);
        
        //Assert
        mockUserRepository.Verify(x => x.Create(It.IsAny<Users>()), Times.Once);
        mockEmailSenderHelper.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        result.Should().NotBeNullOrEmpty();

    }
    
    [Fact]
    public async Task RegisterUser_UserNameExist_ThrowBadRequestException()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
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
        mockUserLoginsRepository.Setup(x => x.CheckIfUserExists(It.IsAny<string>())).ReturnsAsync(true);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.RegisterUser(registerNewUser);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("User with this username already exist");
    }
    
    [Fact]
    public async Task RegisterUser_UserEmail_ThrowBadRequestException()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
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
        mockUserLoginsRepository.Setup(x => x.CheckIfUserExists(It.IsAny<string>())).ReturnsAsync(false);
        mockUserRepository.Setup(x => x.CheckIfUserExist(It.IsAny<string>())).ReturnsAsync(true);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.RegisterUser(registerNewUser);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("User with this email already exist");

    }

    [Fact]
    public async Task GetUser_ForValidParameters_ReturnUserData()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
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
        mockUserLoginsRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(userLogins);
        
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = await userService.GetUser("1");
        
        //Assert
        result.UserName.Should().Be("JoeDeer");
    }

    [Fact]
    public async Task GetUser_ForInvalidUserId_ThrowNotFoundException()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        mockUserLoginsRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((UserLogins)null);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.GetUser("1");
        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
    }

    [Fact]
    public async Task ResendNewConfirmCode_ForValidParameters_ShouldCallUpdateMethod()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var codeConfirmUsers = new CodeConfirmUsers()
        {
            CodeId = "1",
            Code = "1234",
            UserId = "1",
            ExpiryDate = new DateTime(2026, 02, 01)
        };
        var user = new Users()
        {
            UserEmail = "email@gmail.com"
        };
        mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(codeConfirmUsers);
        mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        await userService.ResendNewConfirmCode("1");
        
        //Assert
        mockCodeConfirmUsersRepository.Verify(x => x.Update(It.IsAny<CodeConfirmUsers>()), Times.Once);
    }

    [Fact]
    public async Task ResendNewConfirmCode_ForInvalidParameters_ShouldCallCreateMethod()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var codeConfirmUsers = new CodeConfirmUsers()
        {
            CodeId = "1",
            Code = "1234",
            UserId = "1",
            ExpiryDate = new DateTime(2026, 02, 01)
        };
        var user = new Users()
        {
            UserEmail = "email@gmail.com"
        };
        mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync((CodeConfirmUsers)null);
        mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        await userService.ResendNewConfirmCode("1");
        
        //Assert
        mockCodeConfirmUsersRepository.Verify(x => x.Create(It.IsAny<CodeConfirmUsers>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmUser_ForValidParameters_ShouldUpdateUser()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var confirmCodeDto = new ConfirmCodeDto()
        {
            UserId = "1",
            ConfirmCode = "1234"
        };
        var confirmCode = new CodeConfirmUsers()
        {
            CodeId = "1",
            Code = "1234",
            UserId = "1",
            ExpiryDate = new DateTime(2026, 09, 01)
        };
        var user = new Users()
        {
            IsActive = false
        };
        mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync(confirmCode);
        mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        await userService.ConfirmUser(confirmCodeDto);
        
        //Assert
        mockUserRepository.Verify(x => x.Update(It.IsAny<Users>()), Times.Once);
        user.IsActive.Should().BeTrue();
    }
    
    [Fact]
    public async Task ConfirmUser_ForNullableCodeConfirmUsers_NotFoundException()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var confirmCode = new CodeConfirmUsers()
        {
            CodeId = "1",
            Code = "1234",
            UserId = "1",
            ExpiryDate = new DateTime(2026, 09, 01)
        };
        var confirmCodeDto = new ConfirmCodeDto()
        {
            UserId = "1",
            ConfirmCode = "1234"
        };
        var user = new Users()
        {
            IsActive = false
        };
        mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync((CodeConfirmUsers)null);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.ConfirmUser(confirmCodeDto);
        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("Confirm code not found");
    }
    
    [Fact]
    public async Task ConfirmUser_ExpiryDateIsOlderCurrentTime_BadRequestException()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var confirmCode = new CodeConfirmUsers()
        {
            CodeId = "1",
            Code = "1234",
            UserId = "1",
            ExpiryDate = DateTime.Now.AddDays(-1)
        };
        var confirmCodeDto = new ConfirmCodeDto()
        {
            UserId = "1",
            ConfirmCode = "1234"
        };
        var user = new Users()
        {
            IsActive = false
        };
        mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync(confirmCode);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.ConfirmUser(confirmCodeDto);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Confirm code is expired. You must generate new code");
    }
    
    [Fact]
    public async Task ConfirmUser_CodesAreDifferent_BadRequestException()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var confirmCode = new CodeConfirmUsers()
        {
            CodeId = "1",
            Code = "12345",
            UserId = "1",
            ExpiryDate = DateTime.Now.AddDays(1)
        };
        var confirmCodeDto = new ConfirmCodeDto()
        {
            UserId = "1",
            ConfirmCode = "1234"
        };
        mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync(confirmCode);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.ConfirmUser(confirmCodeDto);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Confirm code is incorrect");
    }
    
    [Fact]
    public async Task ConfirmUser_NullableUser_NotFoundException()
    {
        //Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var confirmCode = new CodeConfirmUsers()
        {
            CodeId = "1",
            Code = "1234",
            UserId = "1",
            ExpiryDate = DateTime.Now.AddDays(1)
        };
        var confirmCodeDto = new ConfirmCodeDto()
        {
            UserId = "1",
            ConfirmCode = "1234"
        };
        mockCodeConfirmUsersRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync(confirmCode);
        mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync((Users)null);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.ConfirmUser(confirmCodeDto);
        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
    }

    [Fact]
    public async Task RecoverPassword_IfRecoveryCodeExist_ShouldCallUpdateCodeMethod()
    {
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var user = new Users()
        {
            UserId = "1"
        };
        var codeRecoveryPassword = new CodeRecoveryPassword()
        {
            CodeId = "1",
            Code = "1234",
            UserId = "1",
            ExpiryDate = new DateTime(2026, 10, 01),
            IsUsed = false
        };
        mockUserRepository.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync(user);
        mockUtilsService.Setup(x => x.GenerateCodeToRecoverPassword()).Returns("1234");
        mockUtilsService.Setup(x => x.GenerateLinkToRecoveryPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("http://localhost:5000/recovery-password/1234");
        mockCodeRecoveryPasswordsRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync(codeRecoveryPassword);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        await userService.RecoverPassword("email@gmail.com");
        
        //Assert
        mockCodeRecoveryPasswordsRepository.Verify(x => x.Update(It.IsAny<CodeRecoveryPassword>()), Times.Once);
        mockEmailSenderHelper.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

    }
    
    [Fact]
    public async Task RecoverPassword_IfRecoveryCodeNotExist_ShouldCallCreateCodeMethod()
    {
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var user = new Users()
        {
            UserId = "1"
        };
        var codeRecoveryPassword = new CodeRecoveryPassword()
        {
            CodeId = "1",
            Code = "1234",
            UserId = "1",
            ExpiryDate = new DateTime(2026, 10, 01),
            IsUsed = false
        };
        mockUserRepository.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync(user);
        mockUtilsService.Setup(x => x.GenerateCodeToRecoverPassword()).Returns("1234");
        mockUtilsService.Setup(x => x.GenerateLinkToRecoveryPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("http://localhost:5000/recovery-password/1234");
        mockCodeRecoveryPasswordsRepository.Setup(x => x.GetByUserId(It.IsAny<string>()))
            .ReturnsAsync((CodeRecoveryPassword)null);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        await userService.RecoverPassword("email@gmail.com");
        
        //Assert
        mockCodeRecoveryPasswordsRepository.Verify(x => x.Create(It.IsAny<CodeRecoveryPassword>()), Times.Once);
        mockEmailSenderHelper.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task RecoverPassword_IfUserNotExist_ThrowBadRequestException()
    {
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        mockUserRepository.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync((Users)null);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.RecoverPassword("email@gmail.com");

        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
    }
    
    [Fact]
    public async Task UpdatePassword_IfPasswordsAreCorrect_ShouldUpdateUserPassword()
    {
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
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
            ConfirmPassword = "password",
            Token = "1234"
        };
        mockUserRepository.Setup(x => x.GetUserWithUserLoginsAndCodeRecovery(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);
        mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<UserLogins>(), It.IsAny<string>())).Returns("hashedPassword");
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        await userService.UpdatePassword(recoveryUpdatePasswordDto);
        
        //Assert
        mockUserRepository.Verify(x => x.Update(It.IsAny<Users>()), Times.Once);
        mockEmailSenderHelper.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        user.UserLogins.Password.Should().Be("hashedPassword");
    }
    
    [Fact]
    public async Task UpdatePassword_PasswordsIsDifferent_ThrowBadRequestException()
    {
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
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
        mockUserRepository.Setup(x => x.GetUserWithUserLoginsAndCodeRecovery(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);
        mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<UserLogins>(), It.IsAny<string>())).Returns("hashedPassword");
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.UpdatePassword(recoveryUpdatePasswordDto);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Passwords are not equal");
    }
        
    [Fact]
    public async Task UpdatePassword_UserNotExist_ThrowBadRequestException()
    {
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
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
            ConfirmPassword = "password",
            Token = "1234"
        };
        mockUserRepository.Setup(x => x.GetUserWithUserLoginsAndCodeRecovery(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((Users)null);
        mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<UserLogins>(), It.IsAny<string>())).Returns("hashedPassword");
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.UpdatePassword(recoveryUpdatePasswordDto);
        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
    }
    
    [Fact]
    public async Task UpdatePassword_DateIsExpired_ThrowBadRequestException()
    {
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
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
        mockUserRepository.Setup(x => x.GetUserWithUserLoginsAndCodeRecovery(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);
        mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<UserLogins>(), It.IsAny<string>())).Returns("hashedPassword");
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.UpdatePassword(recoveryUpdatePasswordDto);
        
        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Recovery code is expired");
    }
    
    [Fact]
    public async Task UpdateUser_IfUserExist_ShouldUpdateUser()
    {
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
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
        mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        await userService.UpdateUser(updateUserDto, "1");
        
        //Assert
        mockUserRepository.Verify(x => x.Update(It.IsAny<Users>()), Times.Once);
        
    }

    [Fact]
    public async Task UpdateUser_IfUserNotExist_ThrowNotFoundException()
    {
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserLoginsRepository = new Mock<IUserLoginsRepository>();
        var mockCodeConfirmUsersRepository = new Mock<ICodeConfirmUsersRepository>();
        var mockCodeRecoveryPasswordsRepository = new Mock<ICodeRecoveryPasswordsRepository>();
        var mockEmailSenderHelper = new Mock<IEmailSenderHelper>();
        var mockPasswordHasher = new Mock<IPasswordHasher<UserLogins>>();
        var mockUtilsService = new Mock<IUtilsService>();
        var updateUserDto = new UpdateUserDto()
        {
            FirstName = "Jan",
            LastName = "Nowak",
            UserEmail = "email@gmail.com"
        };
        mockUserRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync((Users)null);
        
        //Act
        var userService = new UserService(mockPasswordHasher.Object, mockUtilsService.Object, mockEmailSenderHelper.Object, mockUserRepository.Object, mockUserLoginsRepository.Object, mockCodeConfirmUsersRepository.Object, mockCodeRecoveryPasswordsRepository.Object);
        var result = async () => await userService.UpdateUser(updateUserDto, "1");
        
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        
    }

    
    
}
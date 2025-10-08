using System.Security.Claims;
using GameLogBack.Dtos;
using GameLogBack.Dtos.User;
using GameLogBack.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLogBack.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public ActionResult<string> RegisterUser([FromBody] RegisterNewUserDto registerNewUser)
    {
        var userId = _userService.RegisterUser(registerNewUser);
        return Ok(userId);
    }

    [HttpGet("get-user")]
    [Authorize]
    public ActionResult<GetUserDto> GetUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = _userService.GetUser(userId);
        return Ok(user);
    }

    [HttpPut("update")]
    public ActionResult UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _userService.UpdateUser(updateUserDto, userId);
        return Ok();
    }

    [HttpPost("resend-code")]
    public ActionResult ResendCode([FromBody] ResendCodeDto resendCodeDto)
    {
        _userService.ResendNewConfirmCode(resendCodeDto.UserId);
        return Ok();
    }

    [HttpPost("confirm-user")]
    public ActionResult ConfirmUser([FromBody] ConfirmCodeDto confirmCodeDto)
    {
        _userService.ConfirmUser(confirmCodeDto);
        return Ok();
    }
    [HttpPost("recovery-password")]
    public ActionResult RecoverPassword([FromBody] EmailRecoveryPasswordDto userEmail)
    {
        _userService.RecoverPassword(userEmail.UserEmail);
        return Ok();
    }

    [HttpPost("recovery-update-password")]
    public ActionResult RecoveryUpdatePassword([FromBody] RecoveryUpdatePasswordDto recoveryUpdatePasswordDto)
    {
        _userService.RecoveryUpdatePassword(recoveryUpdatePasswordDto);
        return Ok();
    }
}
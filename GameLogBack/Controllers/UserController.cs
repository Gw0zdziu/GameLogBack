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
    public async Task<ActionResult<string>> RegisterUser([FromBody] RegisterNewUserDto registerNewUser)
    {
        var userId = await _userService.RegisterUser(registerNewUser);
        return Ok(userId);
    }

    [HttpGet("get-user")]
    [Authorize]
    public async Task<ActionResult<GetUserDto>> GetUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userService.GetUser(userId);
        return Ok(user);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _userService.UpdateUser(updateUserDto, userId);
        return Ok();
    }

    [HttpPost("resend-code")]
    public async Task<IActionResult> ResendCode([FromBody] ResendCodeDto resendCodeDto)
    {
        await _userService.ResendNewConfirmCode(resendCodeDto.UserId);
        return Ok();
    }

    [HttpPost("confirm-user")]
    public async Task<ActionResult> ConfirmUser([FromBody] ConfirmCodeDto confirmCodeDto)
    {
        await _userService.ConfirmUser(confirmCodeDto);
        return Ok();
    }
    [HttpPost("recovery-password")]
    public async Task<ActionResult> RecoverPassword([FromBody] EmailRecoveryPasswordDto userEmail)
    {
        await _userService.RecoverPassword(userEmail.UserEmail);
        return Ok();
    }

    [HttpPost("recovery-update-password")]
    public async Task<ActionResult> RecoveryUpdatePassword([FromBody] RecoveryUpdatePasswordDto recoveryUpdatePasswordDto)
    {
        await _userService.RecoveryUpdatePassword(recoveryUpdatePasswordDto);
        return Ok();
    }
}
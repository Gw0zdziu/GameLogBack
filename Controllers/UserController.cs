using GameLogBack.Interfaces;
using GameLogBack.Models;
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
    
    [HttpPut("update/{userId}")]
    public ActionResult UpdateUser([FromBody]UpdateUserDto updateUserDto, [FromRoute] string userId)
    {
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
}
using GameLogBack.Interfaces;
using GameLogBack.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameLogBack.Controllers;

[Route("api/account")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public ActionResult RegisterUser([FromBody] RegisterNewUserDto registerNewUser)
    {
        _userService.RegisterUser(registerNewUser);
        return Ok();
    }

    [HttpPost("login")]
    public ActionResult LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        var token = _userService.LoginUser(loginUserDto);
        return Ok(token);
    }
}
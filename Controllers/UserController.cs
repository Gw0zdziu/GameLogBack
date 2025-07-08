using GameLogBack.Authentication;
using GameLogBack.Interfaces;
using GameLogBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLogBack.Controllers;

[Route("api/account")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AuthenticationSettings _authenticationSettings;
    
    public UserController(IUserService userService, AuthenticationSettings authenticationSettings)
    {
        _userService = userService;
        _authenticationSettings = authenticationSettings;
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
        Response.Cookies.Append("refreshToken", token.RefreshToken, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddDays(_authenticationSettings.JwtAccessTokenExpireDays)
        });
        return Ok(token.Token);
    }

    [HttpPost("refresh-token")]
    public ActionResult RefreshToken([FromBody] string accessToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var tokenInfo = new TokenInfoDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        var newTokenInfo = _userService.GetRefreshToken(tokenInfo);
        Response.Cookies.Append("refreshToken", newTokenInfo.RefreshToken, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddDays(_authenticationSettings.JwtAccessTokenExpireDays)
        });
        return Ok(newTokenInfo.AccessToken);   
    }
    
    [HttpGet("confirm-code/{userId}")]
    public ActionResult ConfirmCode([FromRoute] string userId)
    {
        _userService.ResendNewConfirmCode(userId);
        return Ok();   
    }

    [HttpGet("logout")]
    [Authorize]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("refreshToken");
        return Ok();   
    }

    [HttpPatch("update/{userId}")]
    public ActionResult UpdateUser([FromRoute] string userId)
    {
        
        return Ok();  
    }
    
}
using GameLogBack.Authentication;
using GameLogBack.Dtos;
using GameLogBack.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLogBack.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService, AuthenticationSettings authenticationSettings)
    {
        _authService = authService;
        _authenticationSettings = authenticationSettings;
    }

    [HttpPost("login")]
    public ActionResult<string> LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        var token = _authService.LoginUser(loginUserDto);
        Response.Cookies.Append("refreshToken", token.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddDays(_authenticationSettings.JwtAccessTokenExpireDays)
        });
        var login = new
        {
            token.Token,
            token.UserId
        };
        return Ok(login);
    }

    [HttpPost("refresh-token")]
    public ActionResult RefreshToken([FromBody] string accessToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var tokenInfo = new TokenInfoDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        var newTokenInfo = _authService.GetRefreshToken(tokenInfo);
        Response.Cookies.Append("refreshToken", newTokenInfo.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddDays(_authenticationSettings.JwtAccessTokenExpireDays)
        });
        return Ok(newTokenInfo.AccessToken);
    }


    [HttpDelete("logout/{userId}")]
    [Authorize]
    public ActionResult Logout([FromRoute] string userId)
    {
        _authService.LogoutUser(userId);
        Response.Cookies.Delete("refreshToken");
        return Ok();
    }
}
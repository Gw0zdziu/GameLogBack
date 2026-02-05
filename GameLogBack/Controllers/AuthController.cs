using System.Security.Claims;
using GameLogBack.Authentication;
using GameLogBack.Dtos;
using GameLogBack.Dtos.Auth;
using GameLogBack.Exceptions;
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
    public async Task<ActionResult<string>> LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        var token = await _authService.LoginUser(loginUserDto);
        return Ok(token);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var accessToken = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new BadRequestException("Access token is empty");
        }
        accessToken = accessToken.Replace("Bearer ", "");
        var tokenInfo = new TokenInfoDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        var newTokenInfo = await _authService.GetRefreshToken(tokenInfo);
        
         return Ok(newTokenInfo);
    }


    [HttpDelete("logout")] 
    public IActionResult Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _authService.LogoutUser(userId);
        return Ok();
    }
    
}
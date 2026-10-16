using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using GameLogBack.Dtos.Auth;
using GameLogBack.Dtos.Auth.RequestDto;
using GameLogBack.Dtos.Auth.ResponseDto;
using GameLogBack.Dtos.User;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameLogBack.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IOptions<AuthenticationSettings> _authenticationSettings;
    private readonly IAuthService _authService;
    private readonly IValidator<LoginUserDto> _validator;

    public AuthController(IAuthService authService,IValidator<LoginUserDto> validator, IOptions<AuthenticationSettings> authenticationSettings)
    {
        _authService = authService;
        _validator = validator;
        _authenticationSettings = authenticationSettings;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        var validate = await _validator.ValidateAsync(loginUserDto);

        if (!validate.IsValid)
        {
            var errors = validate.Errors.Select(e =>  new {e.PropertyName, Errors = new List<object>(){e.ErrorMessage}});
            return BadRequest(errors);
        }
        
        var result = await _authService.LoginUser(loginUserDto);
        Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = DateTime.UtcNow.AddMinutes(_authenticationSettings.Value.JwtRefreshTokenExpireMinutes),
            Path = "/api/auth"
        });
        var returnValue = new LoginDataResponseDto()
        {
            Token = result.Token,
            ExpiresIn = _authenticationSettings.Value.JwtAccessTokenExpireMinutes,
            User = new GetUserDto()
            {
                UserId = result.User.UserId,
                UserName = result.User.UserName,
                FirstName = result.User.FirstName,
                LastName = result.User.LastName,
                UserEmail = result.User.UserEmail,
                IsActive = result.User.IsActive
            }
        };
        return Ok(returnValue);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<string>> RefreshToken()
    {
        Request.Cookies.TryGetValue("refresh_token", out var refreshToken);
        var newTokenInfo = await _authService.GetRefreshToken(refreshToken);
        return Ok(newTokenInfo);

    }


    [HttpDelete("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _authService.LogoutUser(userId);
        Response.Cookies.Delete("refresh_token", new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Path = "/api/auth"
        });
        return NoContent();
    }
    
    [HttpGet("verify")]
    [Authorize]
    public ActionResult Verify()
    {
        return Ok(true);
    }
}

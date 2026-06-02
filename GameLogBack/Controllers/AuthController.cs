using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using FluentValidation;
using GameLogBack.Dtos.Auth;
using GameLogBack.Dtos.Auth.RequestDto;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLogBack.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IAuthService _authService;
    private readonly IValidator<LoginUserDto> _validator;

    public AuthController(IAuthService authService, AuthenticationSettings authenticationSettings, IValidator<LoginUserDto> validator)
    {
        _authService = authService;
        _authenticationSettings = authenticationSettings;
        _validator = validator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginUser([FromBody] LoginUserDto loginUserDto)
    {
        var result = await _validator.ValidateAsync(loginUserDto);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e =>  new {e.PropertyName, Errors = new List<object>(){e.ErrorMessage}});
            return BadRequest(errors);
        }
        var token = await _authService.LoginUser(loginUserDto);
        return Ok(token);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<string>> RefreshToken()
    {
        var accessToken = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(accessToken)) throw new BadRequestException("Access token is empty");
        accessToken = accessToken.Replace("Bearer ", "");
        var newTokenInfo = await _authService.GetRefreshToken(accessToken);

        return Ok(newTokenInfo);
    }


    [HttpDelete("logout")]
    public IActionResult Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _authService.LogoutUser(userId);
        return Ok();
    }

    [HttpGet("verify")]
    [Authorize]
    public IActionResult Verify()
    {
        return Ok(true);
    }
}

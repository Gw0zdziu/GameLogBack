using System.Security.Claims;
using FluentValidation;
using GameLogBack.Dtos.Game;
using GameLogBack.Dtos.Game.RequestDto;
using GameLogBack.Dtos.Game.ResponseDto;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Interfaces;
using GameLogBack.Validators.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLogBack.Controllers;

[Route("api/games")]
[ApiController]
/*[Authorize]*/
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IValidator<GamePostDto> _gamePostDtoValidator;
    private readonly IValidator<GamePutDto> _gamePutDtoValidator;


    public GameController(IGameService gameService, IValidator<GamePostDto> gamePostDtoValidator, IValidator<GamePutDto> gamePutDtoValidator)
    {
        _gameService = gameService;
        _gamePostDtoValidator = gamePostDtoValidator;
        _gamePutDtoValidator = gamePutDtoValidator;
    }

    [HttpGet("get-games")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGames([FromQuery] PaginatedQuery paginatedQuery)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var games = await _gameService.GetGames(userId, paginatedQuery);
        return Ok(games);
    }

    [HttpGet("get-games_by_categoryId/{categoryId}")]
    public async Task<ActionResult<IEnumerable<GameByUserIdDto>>> GetGamesByCategoryId(
        [FromRoute] string categoryId)
    {
        var games = await _gameService.GetGamesByCategoryId(categoryId);
        return Ok(games);
    }

    [HttpGet("get-games-by-userId/{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesById([FromRoute] string userId)
    {
        var games = await _gameService.GetGamesByUserId(userId);
        return Ok(games);
    }

    [HttpGet("get-game/{gameId}")]
    public async Task<ActionResult<GameDto>> GetGame([FromRoute] string gameId)
    {
        var game = await _gameService.GetGame(gameId);
        return Ok(game);
    }

    [HttpPost("create-game")]
    public async Task<ActionResult<GameDto>> CreateGame([FromBody] GamePostDto newGame)
    {
        var result = await _gamePostDtoValidator.ValidateAsync(newGame);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e =>  new {e.PropertyName, Errors = new List<object>(){e.ErrorMessage}});
            return BadRequest(errors);
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _gameService.PostGame(newGame, userId);
        return Created();
    }

    [HttpPut("update/{gameId}")]
    public async Task<ActionResult<GameDto>> UpdateGame([FromBody] GamePutDto gamePutDto, [FromRoute] string gameId)
    {
        var result = await _gamePutDtoValidator.ValidateAsync(gamePutDto);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e =>  new {e.PropertyName, Errors = new List<object>(){e.ErrorMessage}});
            return BadRequest(errors);
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var game = await _gameService.PutGame(gamePutDto, gameId, userId);
        return Ok(game);
    }

    [HttpDelete("delete/{gameId}")]
    public async Task<IActionResult> DeleteGame([FromRoute] string gameId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _gameService.DeleteGame(gameId, userId);
        return Ok();
    }
}

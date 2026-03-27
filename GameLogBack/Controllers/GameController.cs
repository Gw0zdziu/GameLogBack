using System.Security.Claims;
using GameLogBack.Dtos.Game;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLogBack.Controllers;

[Route("api/games")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("get-games")]
    //[Authorize]
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
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGamesById([FromRoute] string userId)
    {
        var games = await _gameService.GetGamesByUserId(userId);
        return Ok(games);
    }

    [HttpGet("get-game/{gameId}")]
    [Authorize]
    public async Task<ActionResult<GameDto>> GetGame([FromRoute] string gameId)
    {
        var game = await _gameService.GetGame(gameId);
        return Ok(game);
    }

    [HttpPost("create-game")]
    [Authorize]
    public async Task<ActionResult<GameDto>> CreateGame([FromBody] GamePostDto newGame)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var game = await _gameService.PostGame(newGame, userId);
        return Ok(game);
    }

    [HttpPut("update/{gameId}")]
    [Authorize]
    public async Task<ActionResult<GameDto>> UpdateGame([FromBody] GamePutDto gamePutDto, [FromRoute] string gameId)
    {
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
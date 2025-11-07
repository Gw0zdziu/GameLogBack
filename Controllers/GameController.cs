using System.Security.Claims;
using GameLogBack.Dtos.Game;
using GameLogBack.Interfaces;
using GameLogBack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameLogBack.Controllers
{
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
        [Authorize]
        public ActionResult<IEnumerable<GameDto>> GetGames()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var games = _gameService.GetGames(userId);
            return Ok(games);       
        }

        [HttpGet("get-game/{gameId}")]
        [Authorize]
        public ActionResult<GameDto> GetGame(string gameId)
        {
            var game = _gameService.GetGame(gameId);
            return Ok(game);      
        }
        
        [HttpPost("create-game")]
        [Authorize]
        public ActionResult<GameDto> CreateGame([FromBody] GamePostDto newGame)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var game = _gameService.PostGame(newGame, userId);
            return Ok(game);
        }

        [HttpPut("update/{gameId}")]
        [Authorize]
        public ActionResult<GameDto> UpdateGame([FromBody] GamePutDto gamePutDto, [FromRoute] string gameId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var game = _gameService.PutGame(gamePutDto, gameId, userId);
            return Ok(game);
        }

        [HttpDelete("delete/{gameId}")]
        public IActionResult DeleteGame([FromRoute] string gameId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _gameService.DeleteGame(gameId, userId);
            return Ok();      
        }

    }
}

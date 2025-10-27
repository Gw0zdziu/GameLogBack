using System.Security.Claims;
using GameLogBack.Dtos.Game;
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
        public ActionResult CreateGame([FromBody] GamePostDto newGame)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _gameService.PostGame(newGame, userId);
            return Ok();
        }

        [HttpPut("update/{gameId}")]
        [Authorize]
        public ActionResult UpdateGame([FromBody] GamePutDto gamePutDto, [FromRoute] string gameId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _gameService.PutGame(gamePutDto, gameId, userId);
            return Ok();
        }

        [HttpDelete("delete/{gameId}")]
        public ActionResult DeleteGame([FromRoute] string gameId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _gameService.DeleteGame(gameId, userId);
            return Ok();      
        }

    }
}

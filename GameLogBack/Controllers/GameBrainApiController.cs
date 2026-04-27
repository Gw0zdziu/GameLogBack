using GameLogBack.Dtos.GameBrainApi.Response;
using GameLogBack.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace GameLogBack.Controllers;

[ApiController]
[Route("api/gamebrain")]
[AllowAnonymous]
public class GameBrainApiController : ControllerBase
{
    private readonly IGameBrainApiService _gameBrainApiService;

    public GameBrainApiController(IGameBrainApiService gameBrainApiService)
    {
        _gameBrainApiService = gameBrainApiService;
    }

    [HttpGet($"gameName")]
    public async Task<ActionResult<List<GameDetails>>> SearchGameDetails([FromQuery] string gameName)
    {
       return await _gameBrainApiService.SearchGameDetails(gameName);
    }
}

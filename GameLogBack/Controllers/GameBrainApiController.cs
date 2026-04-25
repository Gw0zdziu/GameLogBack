using GameLogBack.Dtos.GameBrainApi.Response;
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
    [HttpGet($"gameName")]
    public async Task<ActionResult<List<GameDetails>>> SearchGameDetails([FromQuery] string gameName)
    {
        var client = new HttpClient();
        const string apiUrl = "https://api.gamebrain.co/v1/games";
        var queryParams = new Dictionary<string, string>
        {
            { "api-key", "7da08660269b46959494cc4eba27b6da" },
            { "query", gameName },
            { "generate-filter-options", "false" }
        };
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        string apiUrL = QueryHelpers.AddQueryString(apiUrl, queryParams);
        try
        {
            var response = await client.GetAsync(apiUrL, cancellationToken: CancellationToken.None);
            string content = response.Content.ReadAsStringAsync().Result;
            var deserializedResult = JsonConvert.DeserializeObject<Games>(content);
            var games = deserializedResult.results.Select(x => new GameDetails()
            {
                name = x.name,
                image = x.image
            }).ToList();
            return Ok(games);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

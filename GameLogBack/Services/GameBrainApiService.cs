using GameLogBack.Dtos.GameBrainApi.Response;
using GameLogBack.Interfaces;
using GameLogBack.Settings;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace GameLogBack.Services;

public class GameBrainApiService : IGameBrainApiService
{
    private readonly HttpClient _httpClient;
    private readonly GameBrainApiSettings _gameBrainApiSettings;

    public GameBrainApiService(HttpClient httpClient, GameBrainApiSettings gameBrainApiSettings)
    {
        _httpClient = httpClient;
        _gameBrainApiSettings = gameBrainApiSettings;
    }

    public async Task<List<GameDetails>> SearchGameDetails(string gameName)
    {
        var queryParams = new Dictionary<string, string>()
        {
            { "api-key", _gameBrainApiSettings.ApiKey },
            { "query", gameName },
            { "generate-filter-options", _gameBrainApiSettings.GenerateFilterOptions }
        };
        var url = QueryHelpers.AddQueryString(_gameBrainApiSettings.ApiUrl, queryParams);
        try
        {
            var response = _httpClient.GetAsync(url);
            var result = await response.Result.Content.ReadAsStringAsync();
            var deserializedResult = JsonConvert.DeserializeObject<GamesBrain>(result);
            var games = deserializedResult.results.Select(x => new GameDetails()
            {
                name = x.name,
                image = x.image
            }).ToList();
            return games;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

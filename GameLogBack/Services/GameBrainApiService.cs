using GameLogBack.Dtos.GameBrainApi.Response;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Settings;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GameLogBack.Services;

public class GameBrainApiService : IGameBrainApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<GameBrainApiSettings> _gameBrainApiSettings;

    public GameBrainApiService(IHttpClientFactory httpClientFactory, IOptions<GameBrainApiSettings> gameBrainApiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _gameBrainApiSettings = gameBrainApiSettings;
    }

    public async Task<List<GameDetails>> SearchGameDetails(string gameName)
    {
        var queryParams = new Dictionary<string, string>()
        {
            { "api-key", _gameBrainApiSettings.Value.ApiKey },
            { "query", gameName },
            { "generate-filter-options", _gameBrainApiSettings.Value.GenerateFilterOptions }
        };

        var url = QueryHelpers.AddQueryString(_gameBrainApiSettings.Value.ApiUrl, queryParams);
    
        try
        {
            var client =  _httpClientFactory.CreateClient("GameBrainApi");
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        
            var resultContent = await response.Content.ReadAsStringAsync();
            var deserializedResult = JsonConvert.DeserializeObject<GameSearchResponse>(resultContent).Results;
        
            var games = deserializedResult.Select(x => new GameDetails()
            {
                Name = x.Name,
                Image = x.Image
            }).ToList();
        
            return games;
        }
        catch (HttpRequestException ex)
        {
            throw new RateLimitExceededException("Your daily points limit of 50 has been reached");
        }
        catch (JsonReaderException ex)
        {
            throw new InvalidOperationException("Failed to parse game data from the API.", ex);
        }
        catch (Exception ex)
        {
            throw new RateLimitExceededException("Your daily points limit of 50 has been reached");
        }
    }
}

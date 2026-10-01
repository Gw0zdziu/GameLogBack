using GameLogBack.Dtos.GameBrainApi.Response;
using GameLogBack.Exceptions;
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
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
        
            var resultContent = await response.Content.ReadAsStringAsync();
            var deserializedResult = JsonConvert.DeserializeObject<GameSearchResponse>(resultContent);
        
            var games = deserializedResult.Results.Select(x => new GameDetails()
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

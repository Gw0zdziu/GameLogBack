using GameLogBack.Dtos.GameBrainApi.Response;

namespace GameLogBack.Interfaces;

public interface IGameBrainApiService
{
    public Task<List<GameDetails>> SearchGameDetails(string gameName);
}

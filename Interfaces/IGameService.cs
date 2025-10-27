using GameLogBack.Dtos.Game;

namespace GameLogBack.Services;

public interface IGameService
{
    IEnumerable<GameDto> GetGames(string userId);
    GameDto GetGame(string gameId);
    void PostGame(GamePostDto gamePostDto, string userId);
    void PutGame(GamePutDto gamePutDto, string gameId, string userId);
    void DeleteGame(string gameId, string userId);
}
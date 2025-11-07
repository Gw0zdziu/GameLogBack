using GameLogBack.Dtos.Game;

namespace GameLogBack.Interfaces;

public interface IGameService
{
    IEnumerable<GameDto> GetGames(string userId);
    GameDto GetGame(string gameId);
    GameDto PostGame(GamePostDto gamePostDto, string userId);
    GameDto PutGame(GamePutDto gamePutDto, string gameId, string userId);
    void DeleteGame(string gameId, string userId);
}
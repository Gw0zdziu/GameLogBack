using GameLogBack.Dtos.Game;

namespace GameLogBack.Interfaces;

public interface IGameService
{
    Task<IEnumerable<GameDto>> GetGames(string userId);
    Task<GameDto> GetGame(string gameId);
    Task<GameDto> PostGame(GamePostDto gamePostDto, string userId);
    Task<GameDto> PutGame(GamePutDto gamePutDto, string gameId, string userId);
    Task DeleteGame(string gameId, string userId);
    Task<IEnumerable<GameByUserIdDto>> GetGamesByUserId(string userId);
}
using GameLogBack.Dtos.Game;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;

namespace GameLogBack.Interfaces;

public interface IGameService
{
    Task<PaginatedResults<GameDto>> GetGames(string userId, PaginatedQuery paginatedQuery);
    Task<GameDto> GetGame(string gameId);
    Task<GameDto> PostGame(GamePostDto gamePostDto, string userId);
    Task<GameDto> PutGame(GamePutDto gamePutDto, string gameId, string userId);
    Task DeleteGame(string gameId, string userId);
    Task<IEnumerable<GameByUserIdDto>> GetGamesByUserId(string userId);
    Task<IEnumerable<GameByCategoryIdDto>> GetGamesByCategoryId(string categoryId);
}
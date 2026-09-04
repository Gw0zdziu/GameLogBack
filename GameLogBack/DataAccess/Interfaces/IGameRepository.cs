using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
using GameLogBack.Entities;

namespace GameLogBack.DataAccess.Interfaces;

public interface IGameRepository
{
    public Task<PaginatedResults<Games>> GetByUserId(string id, PaginatedQuery paginatedQuery);
    public Task<List<Games>> GetByCategoryId(string id, string userId);
    public Task<Games> GetByGameIdAndUserId(string gameName, string userId);
    public Task<bool> CheckIfGameExists(string gameName, string userId);
    public Task<bool> CheckIfGameExitsById(string categoryId);
    public Task<bool> CheckIfExistsWithSameName(string gameName, string userId, string gameId);
    public Task Create(Games game);
    public Task Update(Games game);
    public Task Delete(Games game);
}

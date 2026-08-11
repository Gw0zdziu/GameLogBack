using GameLogBack.Entities;

namespace GameLogBack.DataAccess.Interfaces;

public interface IGameRepository
{
    public IQueryable<Games> GetByUserId(string id);
    public IQueryable<Games> GetById(string id);
    public IQueryable<Games> GetByCategoryId(string id);
    public IQueryable<Games> GetByGameIdAndUserId(string gameName, string userId);
    public Task<bool> CheckIfGameExists(string gameName, string userId);
    public Task<bool> CheckIfExistsWithSameName(string gameName, string userId, string gameId);
    public Task Create(Games game);
    public Task Update(Games game);
    public Task Delete(Games game);
}

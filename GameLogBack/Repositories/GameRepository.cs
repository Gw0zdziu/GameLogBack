using GameLogBack.DbContext;
using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Repositories;


public class GameRepository : IGameRepository
{
    private readonly GameLogDbContext _context;
    public GameRepository(GameLogDbContext context)
    {
        _context = context;
    }
    
    public IQueryable<Games> GetByUserId(string id)
    {
        return _context.Games.Include(x => x.Category).Where(x => x.UserId == id);
    }

    public IQueryable<Games> GetById(string id)
    {
        return _context.Games.Include(x => x.Category).Where(x => x.GameId == id);
    }
    
    public IQueryable<Games> GetByCategoryId(string id)
    {
        return _context.Games.Include(x => x.Category).Where(x => x.CategoryId == id);
    }

    public IQueryable<Games> GetByGameIdAndUserId(string gameId, string userId)
    {
        return _context.Games.Where(x =>
            x.UserId == userId && x.GameId == gameId);
    }

    public async Task<bool> CheckIfGameExists(string gameName, string userId)
    {
        return await _context.Games.AnyAsync(x => x.UserId == userId && x.GameName.Equals(gameName, StringComparison.CurrentCultureIgnoreCase));
    }

    public async Task<bool> CheckIfExistsWithSameName(string gameName, string userId, string gameId)
    {
        return await _context.Games.AnyAsync(x =>
            x.GameId != gameId && x.UserId == userId && x.GameName == gameName);
    }

    public async Task Create(Games game)
    {
        _context.Games.Add(game);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Games game)
    {
        _context.Games.Update(game);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Games game)
    {
        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
    }
}
using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.DataAccess.Repositories;


public class GameRepository : IGameRepository
{
    private readonly GameLogDbContext _context;
    public GameRepository(GameLogDbContext context)
    {
        _context = context;
    }

    public async Task<List<Games>> GetByUserId(string id)
    {
        return await _context.Games.Include(x => x.Category).Where(x => x.UserId == id).ToListAsync();
    }

    public async Task<Games> GetById(string id)
    {
        return await _context.Games.Include(x => x.Category).Where(x => x.GameId == id).FirstOrDefaultAsync();
    }

    public Task<List<Games>> GetByCategoryId(string id)
    {
        return _context.Games.Include(x => x.Category).Where(x => x.CategoryId == id).ToListAsync();
    }

    public async Task<Games> GetByGameIdAndUserId(string gameId, string userId)
    {
        return await _context.Games.Where(x =>
            x.UserId == userId && x.GameId == gameId).FirstOrDefaultAsync();
    }

    public async Task<bool> CheckIfGameExists(string gameName, string userId)
    {
        return await _context.Games.AnyAsync(x => x.UserId == userId && x.GameName.ToLower() == gameName.ToLower());
    }

    public async Task<bool> CheckIfGameExitsById(string gameId)
    {
        return await _context.Games.AnyAsync(x => x.GameId == gameId );
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

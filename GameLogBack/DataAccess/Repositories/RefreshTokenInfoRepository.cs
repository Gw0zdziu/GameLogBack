using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.DataAccess.Repositories;

public class RefreshTokenInfoRepository : IRefreshTokenInfoRepository
{
    private readonly GameLogDbContext _context;

    public RefreshTokenInfoRepository(GameLogDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshTokenInfo> GetByUserId(string userId)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public Task Create(RefreshTokenInfo refreshTokenInfo)
    {
        _context.RefreshTokens.Add(refreshTokenInfo);
        return _context.SaveChangesAsync();
    }

    public Task Update(RefreshTokenInfo refreshTokenInfo)
    {
        _context.RefreshTokens.Update(refreshTokenInfo);
        return _context.SaveChangesAsync();
    }

    public async Task Delete(RefreshTokenInfo refreshTokenInfo)
    {
        _context.RefreshTokens.Remove(refreshTokenInfo);
        await _context.SaveChangesAsync();
    }
}

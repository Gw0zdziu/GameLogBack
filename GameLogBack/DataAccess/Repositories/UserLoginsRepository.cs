using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.DataAccess.Repositories;

public class UserLoginsRepository : IUserLoginsRepository
{
    private readonly GameLogDbContext _context;

    public UserLoginsRepository(GameLogDbContext context)
    {
        _context = context;
    }

    public Task<UserLogins> GetByUserId(string id)
    {
        return _context.UserLogins.Include(x => x.User).Where(x => x.UserId == id).FirstOrDefaultAsync();
    }

    public async Task<bool> CheckIfUserExists(string userName)
    {
        return await _context.UserLogins.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
    }

    public async Task<UserLogins> GetByUserName(string userName)
    {
        return await _context.UserLogins.FirstOrDefaultAsync(x => x.UserName == userName);
    }
}

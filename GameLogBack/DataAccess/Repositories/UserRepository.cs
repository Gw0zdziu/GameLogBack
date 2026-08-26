using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GameLogDbContext _context;

    public UserRepository(GameLogDbContext context)
    {
        _context = context;
    }

    public Task<bool> CheckIfUserExist(string email)
    {
        return _context.Users.AnyAsync(x => x.UserEmail.ToLower() == email);
    }

    public async Task<Users> GetByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.UserEmail == email);
    }

    public async Task<Users> GetById(string userId)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<Users> GetUserWithUserLoginsAndCodeRecovery(string userId, string token)
    {
        return await _context.Users
            .Include(x => x.UserLogins)
            .Include(x => x.CodeRecoveryPassword)
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.CodeRecoveryPassword.Code == token);
    }

    public async Task<Users> GetUserWithConfirmCode(string userId)
    {
        return await _context.Users
            .Include(u => u.CodeConfirm)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }


    public async Task Create(Users user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Users user)
    {
        _context.Update(user);
        await _context.SaveChangesAsync();
    }
}

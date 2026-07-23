using GameLogBack.DbContext;
using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GameLogDbContext _context;
    
    public UserRepository(GameLogDbContext context)
    {
        _context = context;
    }
    
    public Task<bool> CheckIfUserExist(string email)
    {
        return _context.Users.AnyAsync(x => x.UserEmail.Equals(email, StringComparison.CurrentCultureIgnoreCase));
    }

    public async Task<Users> GetByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.UserEmail == email);
    }

    public async Task<Users> GetById(string id)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);
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
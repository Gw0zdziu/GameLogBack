using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.DataAccess.Repositories;

public class CodeConfirmUsersRepository : ICodeConfirmUsersRepository
{
    private readonly GameLogDbContext _context;

    public CodeConfirmUsersRepository(GameLogDbContext context)
    {
        _context = context;
    }

    public async Task<CodeConfirmUsers> GetByUserId(string userId)
    {
        return await _context.CodeConfirmUsers
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task Create(CodeConfirmUsers codeConfirmUsers)
    { 
        _context.CodeConfirmUsers.Update(codeConfirmUsers);
        await _context.SaveChangesAsync();
    }

    public async Task Update(CodeConfirmUsers codeConfirmUsers)
    {
        await _context.CodeConfirmUsers.AddAsync(codeConfirmUsers);
        await _context.SaveChangesAsync();
    }
}

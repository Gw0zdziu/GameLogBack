using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.DataAccess.Repositories;

public class CodeRecoveryPasswordsRepository : ICodeRecoveryPasswordsRepository
{
    private readonly GameLogDbContext _context;

    public CodeRecoveryPasswordsRepository(GameLogDbContext context)
    {
        _context = context;
    }

    public async Task<CodeRecoveryPassword> GetByUserId(string userId)
    {
        return await _context.CodeRecoveryPasswords.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task Create(CodeRecoveryPassword codeRecoveryPassword)
    {
        await _context.CodeRecoveryPasswords.AddAsync(codeRecoveryPassword);
        await _context.SaveChangesAsync();
    }

    public async Task Update(CodeRecoveryPassword codeRecoveryPassword)
    {
        _context.CodeRecoveryPasswords.Update(codeRecoveryPassword);
        await _context.SaveChangesAsync();
    }
}

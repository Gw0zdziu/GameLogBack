using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.DataAccess.Repositories;

public class CodeConfirmUsersRepository : ICodeConfirmUsersRepository
{
    private readonly DbSet<CodeConfirmUsers> _codeConfirmUsers;

    public CodeConfirmUsersRepository(GameLogDbContext context)
    {
        _codeConfirmUsers = context.CodeConfirmUsers;
    }

    public async Task<CodeConfirmUsers> GetByUserId(string userId)
    {
        return await _codeConfirmUsers
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }
}

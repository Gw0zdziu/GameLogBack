using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.DataAccess.Repositories;

public class InvitationCodesRepository : IInvitationCodesRepository
{
    private readonly GameLogDbContext _context;

    public InvitationCodesRepository(GameLogDbContext context)
    {
        _context = context;
    }


    public Task<InvitationCodes> GetActiveInvitationCode(string code)
    {
        return _context.InvitationCodes.FirstOrDefaultAsync(x => x.InvitationCode == code && !x.IsUsed);
    }

    public async Task Update(InvitationCodes invitationCodes)
    {
        _context.Update(invitationCodes);
        await _context.SaveChangesAsync();
    }

    public async Task Create(InvitationCodes invitationCodes)
    {
        await _context.AddAsync(invitationCodes);
        await _context.SaveChangesAsync();
    }
}
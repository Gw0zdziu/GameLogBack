using GameLogBack.Entities;

namespace GameLogBack.DataAccess.Interfaces;

public interface IInvitationCodesRepository
{
    public Task<InvitationCodes> GetActiveInvitationCode(string code);
    public Task Update(InvitationCodes invitationCodes);
    public Task Create(InvitationCodes invitationCodes);

}
using GameLogBack.Entities;

namespace GameLogBack.DataAccess.Interfaces;

public interface ICodeConfirmUsersRepository
{
    public Task<CodeConfirmUsers> GetByUserId(string userId);
}

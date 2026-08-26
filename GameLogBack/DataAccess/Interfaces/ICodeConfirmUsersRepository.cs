using GameLogBack.Entities;

namespace GameLogBack.DataAccess.Interfaces;

public interface ICodeConfirmUsersRepository
{
    public Task<CodeConfirmUsers> GetByUserId(string userId);
    public Task Create(CodeConfirmUsers codeConfirmUsers);
    public Task Update(CodeConfirmUsers codeConfirmUsers);
}

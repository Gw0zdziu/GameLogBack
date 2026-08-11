using GameLogBack.Entities;

namespace GameLogBack.DataAccess.Interfaces;

public interface ICodeRecoveryPasswordsRepository
{
    public Task<CodeRecoveryPassword> GetByUserId(string userId);
    public Task Create(CodeRecoveryPassword codeRecoveryPassword);
}

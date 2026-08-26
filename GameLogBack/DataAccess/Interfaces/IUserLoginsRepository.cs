using GameLogBack.Entities;

namespace GameLogBack.DataAccess.Interfaces;

public interface IUserLoginsRepository
{
    public Task<UserLogins>  GetByUserId(string id);
    public Task<bool> CheckIfUserExists(string userName);
    public Task<UserLogins> GetByUserName(string userName);
}

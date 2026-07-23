using GameLogBack.Entities;

namespace GameLogBack.Repositories;

public interface IUserLoginsRepository
{
    public IQueryable<UserLogins>  GetByUserId(string id);
    public Task<bool> CheckIfUserExists(string userName);
}
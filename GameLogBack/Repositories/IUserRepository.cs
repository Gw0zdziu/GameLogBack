using GameLogBack.Entities;

namespace GameLogBack.Repositories;

public interface IUserRepository
{
    public Task<bool> CheckIfUserExist(string email);
    public Task<Users> GetByEmail(string email);
    public Task<Users> GetById(string Id);
    public Task<Users> GetUserWithUserLoginsAndCodeRecovery(string userId, string token);
    public Task<Users> GetUserWithConfirmCode(string userId);
    public Task Create(Users user);
    public Task Update(Users user);
}
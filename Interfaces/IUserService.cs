using GameLogBack.Models;

namespace GameLogBack.Interfaces;

public interface IUserService
{
    public void RegisterUser(RegisterNewUserDto registerNewUser);
    public string LoginUser(LoginUserDto loginUserDto);
}
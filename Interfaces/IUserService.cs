using GameLogBack.Models;

namespace GameLogBack.Interfaces;

public interface IUserService
{
    public void RegisterUser(RegisterNewUserDto registerNewUser);
    public LoginResponseDto LoginUser(LoginUserDto loginUserDto);
}
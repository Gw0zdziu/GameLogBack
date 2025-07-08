using GameLogBack.Entities;
using GameLogBack.Models;

namespace GameLogBack.Interfaces;

public interface IUserService
{
    public void RegisterUser(RegisterNewUserDto registerNewUser);
    public LoginResponseDto LoginUser(LoginUserDto loginUserDto);
    public TokenInfoDto GetRefreshToken(TokenInfoDto tokenInfo);
    public void ResendNewConfirmCode(string userId);
}
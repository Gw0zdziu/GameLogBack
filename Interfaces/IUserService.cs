using GameLogBack.Entities;
using GameLogBack.Models;

namespace GameLogBack.Interfaces;

public interface IUserService
{
    public string RegisterUser(RegisterNewUserDto registerNewUser);
    public LoginResponseDto LoginUser(LoginUserDto loginUserDto);
    public TokenInfoDto GetRefreshToken(TokenInfoDto tokenInfo);
    public void LogoutUser(string userId);
    public void UpdateUser(UpdateUserDto updateUserDto, string userId);

    public void ResendNewConfirmCode(string userId);
    public void ConfirmUser(ConfirmCodeDto confirmCodeDto);
}
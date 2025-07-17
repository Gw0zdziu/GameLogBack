using GameLogBack.Models;

namespace GameLogBack.Interfaces;

public interface IAuthService
{
    public LoginResponseDto LoginUser(LoginUserDto loginUserDto);
    public TokenInfoDto GetRefreshToken(TokenInfoDto tokenInfo);
    public void LogoutUser(string userId);
}
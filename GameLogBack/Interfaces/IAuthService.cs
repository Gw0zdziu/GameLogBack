using GameLogBack.Dtos;
using GameLogBack.Dtos.Auth;

namespace GameLogBack.Interfaces;

public interface IAuthService
{
    public Task<string> LoginUser(LoginUserDto loginUserDto);
    public Task<string> GetRefreshToken(TokenInfoDto tokenInfo);
    public Task LogoutUser(string userId);
}
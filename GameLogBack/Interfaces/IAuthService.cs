using GameLogBack.Dtos;
using GameLogBack.Dtos.Auth;

namespace GameLogBack.Interfaces;

public interface IAuthService
{
    public Task<LoginResponseDto> LoginUser(LoginUserDto loginUserDto);
    public Task<TokenInfoDto> GetRefreshToken(TokenInfoDto tokenInfo);
    public Task LogoutUser(string userId);
}
using GameLogBack.Dtos;
using GameLogBack.Dtos.Auth;
using GameLogBack.Dtos.Auth.RequestDto;
using GameLogBack.Dtos.Auth.ResponseDto;

namespace GameLogBack.Interfaces;

public interface IAuthService
{
    public Task<LoginResponseDto> LoginUser(LoginUserDto loginUserDto);
    public Task<string> GetRefreshToken(string tokenInfo);
    public Task LogoutUser(string userId);
}
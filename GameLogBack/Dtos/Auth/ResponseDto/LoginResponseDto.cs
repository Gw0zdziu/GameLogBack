using GameLogBack.Dtos.User;

namespace GameLogBack.Dtos.Auth.ResponseDto;

public class LoginResponseDto
{
    public string Token { get; set; }
    public int ExpiresIn { get; set; }
    public GetUserDto User { get; set; }
}
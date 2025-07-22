namespace GameLogBack.Dtos;

public class LoginResponseDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string UserId { get; set; }
}
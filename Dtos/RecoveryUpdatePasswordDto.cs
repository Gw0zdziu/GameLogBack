namespace GameLogBack.Dtos;

public class RecoveryUpdatePasswordDto
{
    public string UserId { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public string Token { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Dtos.Auth;

public class TokenInfoDto
{
    [Required(ErrorMessage = "AccessToken is required")]
    public string AccessToken { get; set; }

    [Required(ErrorMessage = "RefreshToken is required")]
    public string RefreshToken { get; set; }
}
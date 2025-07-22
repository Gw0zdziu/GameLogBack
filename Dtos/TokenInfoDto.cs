using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Dtos;

public class TokenInfoDto
{
    [Required(ErrorMessage = "AccessToken is required")]
    public string AccessToken { get; set; }

    [Required(ErrorMessage = "RefreshToken is required")]
    public string RefreshToken { get; set; }
}
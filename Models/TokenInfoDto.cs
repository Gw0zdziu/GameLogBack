using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Models;

public class TokenInfoDto
{
    [Required(ErrorMessage = "AccessToken is required")]
    public string AccessToken { get; set; }
    
    [Required(ErrorMessage = "RefreshToken is required")]
    public string RefreshToken { get; set; }
}
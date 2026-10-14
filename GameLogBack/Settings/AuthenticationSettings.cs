
using System.ComponentModel.DataAnnotations;

namespace GameLogBack.Settings;

public class AuthenticationSettings
{
    public const string SectionName = "Authentication";

    [Required] 
    public string JwtKey { get; set; } = string.Empty;

    [Required] 
    public int JwtAccessTokenExpireMinutes { get; set; }

    [Required] 
    public int JwtRefreshTokenExpireMinutes { get; set; }
    
    [Required]
    public string JwtIssuer { get; set; }
}

namespace GameLogBack.Authentication;

public class AuthenticationSettings
{
    public string JwtKey { get; set; }
    public int JwtTokenExpireMinutes { get; set; }
    public int JwtAccessTokenExpireDays { get; set; }
    public string JwtIssuer { get; set; }
}
namespace GameLogBack.Settings;

public class AuthenticationSettings
{
    public string JwtKey { get; set; }
    public int JwtAccessTokenExpireMinutes { get; set; }
    public int JwtRefreshTokenExpireMinutes { get; set; }
    public string JwtIssuer { get; set; }
}

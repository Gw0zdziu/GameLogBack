using System.Security.Cryptography;

namespace GameLogBack.Services;

public class UtilsService : IUtilsService
{
    public string GetRefreshToken()
    {
        var refreshToken = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(refreshToken);
        return Convert.ToBase64String(refreshToken);
    }
}
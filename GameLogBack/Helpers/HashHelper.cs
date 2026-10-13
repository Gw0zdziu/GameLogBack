using System.Security.Cryptography;
using System.Text;

namespace GameLogBack.Helpers;

public static class HashHelper
{
    public static string GetHashValue(string value)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }

    public static bool IsHashEqual(string hashedToken, string storedToken)
    {
        var storedHashBytes = Convert.FromHexString(storedToken);
        var computedHashBytes = Convert.FromHexString(hashedToken);
        return CryptographicOperations.FixedTimeEquals(computedHashBytes, storedHashBytes);
    }
}
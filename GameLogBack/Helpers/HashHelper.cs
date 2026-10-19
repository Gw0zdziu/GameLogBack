using System.Security.Cryptography;
using System.Text;

namespace GameLogBack.Helpers;

public static class HashHelper
{
    public static string GetHashValue(string value)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }
    
}
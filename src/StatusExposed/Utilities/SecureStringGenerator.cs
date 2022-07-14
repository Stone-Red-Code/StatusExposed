using System.Security.Cryptography;

namespace StatusExposed.Utilities;

public static class SecureStringGenerator
{
    public static string CreateCryptographicRandomString(int count)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(count));
    }
}
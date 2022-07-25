using System.Security.Cryptography;

namespace StatusExposed.Utilities;

public static class SecureStringGenerator
{
    public static string CreateCryptographicRandomString(int count)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(count));
    }

    public static string CreateCryptographicRandomString(int count, byte uid)
    {
        List<byte> bytes = RandomNumberGenerator.GetBytes(count).ToList();

        bytes.Add(uid);

        return Convert.ToBase64String(bytes.ToArray());
    }
}
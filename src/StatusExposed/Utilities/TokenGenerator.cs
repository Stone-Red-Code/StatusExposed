namespace StatusExposed.Utilities;

public static class TokenGenerator
{
    public static string GenerateToken(string tokenType, int uid = 0, int length = 128)
    {
        return tokenType + "-" + SecureStringGenerator.CreateCryptographicRandomString(length, Convert.ToByte(uid));
    }

    public static bool ValidateToken(string? token, string tokenType)
    {
        if (token is null)
        {
            return false;
        }

        return token.StartsWith(tokenType + "-");
    }
}
using System.Security.Cryptography;

namespace Nautilus.Api.Infrastructure.Email;

public static class TokenGenerator
{
    /// <summary>
    /// Generates a cryptographically secure random token
    /// </summary>
    public static string GenerateToken(int length = 32)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "")
            .Substring(0, length);
    }
}

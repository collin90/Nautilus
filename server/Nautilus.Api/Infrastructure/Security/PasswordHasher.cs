using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Nautilus.Api.Infrastructure.Security;

public class PasswordHasher
{
    private const int SaltSize = 32;   // 256-bit salt
    private const int KeySize = 32;    // 256-bit hash
    private const int Iterations = 210_000;

    public (string Hash, string Salt) Hash(string password)
    {
        // generate random salt
        byte[] saltBytes = new byte[SaltSize];
        RandomNumberGenerator.Fill(saltBytes);

        // PBKDF2 hash
        byte[] hashBytes = KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: Iterations,
            numBytesRequested: KeySize
        );

        return (
            Hash: Convert.ToBase64String(hashBytes),
            Salt: Convert.ToBase64String(saltBytes)
        );
    }

    public bool Verify(string password, string storedHash, string storedSalt)
    {
        byte[] saltBytes = Convert.FromBase64String(storedSalt);

        byte[] computedHash = KeyDerivation.Pbkdf2(
            password,
            saltBytes,
            KeyDerivationPrf.HMACSHA256,
            Iterations,
            KeySize
        );

        return CryptographicOperations.FixedTimeEquals(
            computedHash,
            Convert.FromBase64String(storedHash)
        );
    }
}

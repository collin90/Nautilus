using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Nautilus.Api.Infrastructure.Security;

public class PasswordHasher
{
    private const int SaltSize = 32;   // 256-bit salt
    private const int KeySize = 32;    // 256-bit hash
    private const int Iterations = 210_000;

    public (byte[] Hash, byte[] Salt) Hash(string password)
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
            Hash: hashBytes,
            Salt: saltBytes
        );
    }

    public bool Verify(string password, byte[] storedHash, byte[] storedSalt)
    {
        byte[] computedHash = KeyDerivation.Pbkdf2(
            password,
            storedSalt,
            KeyDerivationPrf.HMACSHA256,
            Iterations,
            KeySize
        );

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}

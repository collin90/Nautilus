using Nautilus.Api.Services;
using Nautilus.Api.Infrastructure.Security;
using Nautilus.Api.Domain.Users;

namespace Nautilus.Api.Infrastructure.Auth;

// Database-backed implementation of IAuthRepository (keeps existing behavior)
public class AuthRepositoryDB(IDatabaseClient db, PasswordHasher passwordHasher) : IAuthRepository
{
    private readonly IDatabaseClient _db = db;
    private readonly PasswordHasher _passwordHasher = passwordHasher;

    // Stored Procedure Names
    private const string RegisterUserProcedure = "auth.RegisterUser";
    private const string LoginUserProcedure = "auth.LoginUser";

    public async Task<Guid?> RegisterAsync(string userName, string email, string password)
    {
        // Generate salt + hash
        var (hash, salt) = _passwordHasher.Hash(password);

        var result = await _db.QuerySingleAsync<Guid?>(
            RegisterUserProcedure,
            new { UserName = userName, Email = email, PasswordHash = hash, PasswordSalt = salt }
        );

        return result;
    }

    public async Task<User?> GetUserByIdentifierAsync(string identifier)
    {
        return await _db.QuerySingleAsync<User>(
            LoginUserProcedure,
            new { UserNameOrEmail = identifier }
        );
    }
}

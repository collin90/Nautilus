using Dapper;
using Nautilus.Api.Services;
using Nautilus.Api.Infrastructure.Security;
using Nautilus.Api.Domain.Users;

using System.Data;

namespace Nautilus.Api.Infrastructure;

public class AuthRepository
{
    private readonly IDatabaseClient _db;
    private readonly PasswordHasher _passwordHasher;

    // Stored Procedure Names 
    private const string RegisterUserProcedure = "auth.RegisterUser";
    private const string LoginUserProcedure = "auth.GetUserByEmailOrUsername";

    public AuthRepository(IDatabaseClient db, PasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

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

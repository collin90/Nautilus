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

    public async Task<bool> SetActivationTokenAsync(Guid userId, string token, DateTime expiry)
    {
        // TODO: Implement database stored procedure call
        // For now, return true as a placeholder
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ActivateUserAsync(string token)
    {
        // TODO: Implement database stored procedure call
        // For now, return true as a placeholder
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> SetPasswordResetTokenAsync(Guid userId, string token, DateTime expiry)
    {
        // TODO: Implement database stored procedure call
        // For now, return true as a placeholder
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword, PasswordHasher hasher)
    {
        // TODO: Implement database stored procedure call
        // For now, return true as a placeholder
        await Task.CompletedTask;
        return true;
    }
}

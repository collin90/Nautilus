
using Nautilus.Api.Domain.Users;
using Nautilus.Api.Infrastructure.Security;

namespace Nautilus.Api.Infrastructure.Auth;

// In-memory implementation of IAuthRepository for local testing
public class AuthRepositoryMem(PasswordHasher passwordHasher, ILogger<AuthRepositoryMem> logger) : IAuthRepository
{
    private readonly PasswordHasher _passwordHasher = passwordHasher;
    private readonly ILogger<AuthRepositoryMem> _logger = logger;
    // Use shared in-memory user store
    private static List<User> Users => Shared.UserStore.Users;

    public Task<Guid?> RegisterAsync(string userName, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("Register called with empty email");
            return Task.FromResult<Guid?>(null);
        }

        if (Users.Any(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Register failed - user already exists: {Email}", email);
            return Task.FromResult<Guid?>(null);
        }

        var (hash, salt) = _passwordHasher.Hash(password);

        var user = new User
        {
            UserId = Guid.NewGuid(),
            UserName = userName,
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActivated = false
        };

        Users.Add(user);

        _logger.LogInformation("Registered user {UserId} {Email} {UserName}", user.UserId, user.Email, user.UserName);

        return Task.FromResult<Guid?>(user.UserId);
    }

    public Task<User?> GetUserByIdentifierAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            _logger.LogWarning("GetUserByIdentifier called with empty identifier");
            return Task.FromResult<User?>(null);
        }

        var user = Users.FirstOrDefault(u => string.Equals(u.Email, identifier, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(u.UserName, identifier, StringComparison.OrdinalIgnoreCase));

        if (user is null)
            _logger.LogWarning("User not found for identifier: {Identifier}", identifier);
        else
            _logger.LogInformation("Found user {UserId} {Email} {UserName} for identifier {Identifier}", user.UserId, user.Email, user.UserName, identifier);

        return Task.FromResult(user);
    }

    public Task<bool> SetActivationTokenAsync(Guid userId, string token, DateTime expiry)
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);
        if (user is null)
        {
            _logger.LogWarning("SetActivationToken failed - user not found: {UserId}", userId);
            return Task.FromResult(false);
        }

        user.ActivationToken = token;
        user.ActivationTokenExpiry = expiry;
        user.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Activation token set for user {UserId}: Token={Token}, Expiry={Expiry}", userId, token, expiry);
        return Task.FromResult(true);
    }

    public Task<bool> ActivateUserAsync(string token)
    {
        _logger.LogDebug("ActivateUserAsync called with token: {Token}", token);
        _logger.LogDebug("Current users in store: {Count}", Users.Count);

        foreach (var u in Users)
        {
            _logger.LogDebug("User {UserId} has ActivationToken: {Token}", u.UserId, u.ActivationToken ?? "NULL");
        }

        var user = Users.FirstOrDefault(u => u.ActivationToken == token);
        if (user is null)
        {
            _logger.LogWarning("ActivateUser failed - invalid token: {Token}", token);
            return Task.FromResult(false);
        }

        if (user.ActivationTokenExpiry < DateTime.UtcNow)
        {
            _logger.LogWarning("ActivateUser failed - token expired for user {UserId}", user.UserId);
            return Task.FromResult(false);
        }

        user.IsActivated = true;
        user.ActivationToken = null;
        user.ActivationTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("User activated: {UserId} {Email}", user.UserId, user.Email);
        return Task.FromResult(true);
    }

    public Task<bool> SetPasswordResetTokenAsync(Guid userId, string token, DateTime expiry)
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);
        if (user is null)
        {
            _logger.LogWarning("SetPasswordResetToken failed - user not found: {UserId}", userId);
            return Task.FromResult(false);
        }

        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiry = expiry;
        user.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Password reset token set for user {UserId}", userId);
        return Task.FromResult(true);
    }

    public Task<bool> ResetPasswordAsync(string token, string newPassword, PasswordHasher hasher)
    {
        var user = Users.FirstOrDefault(u => u.PasswordResetToken == token);
        if (user is null)
        {
            _logger.LogWarning("ResetPassword failed - invalid token");
            return Task.FromResult(false);
        }

        if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            _logger.LogWarning("ResetPassword failed - token expired for user {UserId}", user.UserId);
            return Task.FromResult(false);
        }

        // Hash new password
        var (hash, salt) = hasher.Hash(newPassword);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Password reset successfully for user {UserId}", user.UserId);
        return Task.FromResult(true);
    }
}

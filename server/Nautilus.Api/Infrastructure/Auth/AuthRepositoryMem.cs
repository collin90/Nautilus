
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
            IsActive = true
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
}

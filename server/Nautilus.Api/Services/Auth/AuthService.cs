using Nautilus.Api.Backend;
using Nautilus.Api.Data;

namespace Nautilus.Api.Services.Auth;

/// <summary>
/// Authentication service that delegates to backend implementation
/// </summary>
public class AuthService(IAuthBackend backend, ILogger<AuthService> logger) : IAuthService
{
    private readonly IAuthBackend _backend = backend;
    private readonly ILogger<AuthService> _logger = logger;

    public Task<Guid?> RegisterAsync(string userName, string email, string password)
    {
        _logger.LogDebug("AuthService.RegisterAsync called for email: {Email}", email);
        return _backend.RegisterAsync(userName, email, password);
    }

    public Task<User?> GetUserByIdentifierAsync(string identifier)
    {
        _logger.LogDebug("AuthService.GetUserByIdentifierAsync called for identifier: {Identifier}", identifier);
        return _backend.GetUserByIdentifierAsync(identifier);
    }

    public Task<bool> SetActivationTokenAsync(Guid userId, string token, DateTime expiry)
    {
        _logger.LogDebug("AuthService.SetActivationTokenAsync called for user: {UserId}", userId);
        return _backend.SetActivationTokenAsync(userId, token, expiry);
    }

    public Task<bool> ActivateUserAsync(string token)
    {
        _logger.LogDebug("AuthService.ActivateUserAsync called");
        return _backend.ActivateUserAsync(token);
    }

    public Task<bool> SetPasswordResetTokenAsync(Guid userId, string token, DateTime expiry)
    {
        _logger.LogDebug("AuthService.SetPasswordResetTokenAsync called for user: {UserId}", userId);
        return _backend.SetPasswordResetTokenAsync(userId, token, expiry);
    }

    public Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        _logger.LogDebug("AuthService.ResetPasswordAsync called");
        return _backend.ResetPasswordAsync(token, newPassword);
    }
}

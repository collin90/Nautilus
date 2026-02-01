using Nautilus.Api.Data;

namespace Nautilus.Api.Backend.Sql;

/// <summary>
/// Database implementation of authentication backend (TODO: Implement)
/// </summary>
public class AuthBackend(ILogger<AuthBackend> logger) : IAuthBackend
{
    private readonly ILogger<AuthBackend> _logger = logger;

    public async Task<Guid?> RegisterAsync(string userName, string email, string password)
    {
        // TODO: Implement database stored procedure call
        _logger.LogWarning("AuthBackend.RegisterAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return null;
    }

    public async Task<User?> GetUserByIdentifierAsync(string identifier)
    {
        // TODO: Implement database stored procedure call
        _logger.LogWarning("AuthBackend.GetUserByIdentifierAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return null;
    }

    public async Task<bool> SetActivationTokenAsync(Guid userId, string token, DateTime expiry)
    {
        // TODO: Implement database stored procedure call
        _logger.LogWarning("AuthBackend.SetActivationTokenAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return false;
    }

    public async Task<bool> ActivateUserAsync(string token)
    {
        // TODO: Implement database stored procedure call
        _logger.LogWarning("AuthBackend.ActivateUserAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return false;
    }

    public async Task<bool> SetPasswordResetTokenAsync(Guid userId, string token, DateTime expiry)
    {
        // TODO: Implement database stored procedure call
        _logger.LogWarning("AuthBackend.SetPasswordResetTokenAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return false;
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        // TODO: Implement database stored procedure call
        _logger.LogWarning("AuthBackend.ResetPasswordAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return false;
    }
}

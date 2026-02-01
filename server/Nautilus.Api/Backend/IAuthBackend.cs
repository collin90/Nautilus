using Nautilus.Api.Data;

namespace Nautilus.Api.Backend;

/// <summary>
/// Backend interface for authentication operations
/// </summary>
public interface IAuthBackend
{
    Task<Guid?> RegisterAsync(string userName, string email, string password);
    Task<User?> GetUserByIdentifierAsync(string identifier);
    Task<bool> SetActivationTokenAsync(Guid userId, string token, DateTime expiry);
    Task<bool> ActivateUserAsync(string token);
    Task<bool> SetPasswordResetTokenAsync(Guid userId, string token, DateTime expiry);
    Task<bool> ResetPasswordAsync(string token, string newPassword);
}

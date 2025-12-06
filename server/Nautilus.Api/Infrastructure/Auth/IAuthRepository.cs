using Nautilus.Api.Domain.Users;
using Nautilus.Api.Infrastructure.Security;

namespace Nautilus.Api.Infrastructure.Auth;

public interface IAuthRepository
{
    Task<Guid?> RegisterAsync(string userName, string email, string password);
    Task<User?> GetUserByIdentifierAsync(string identifier);
    Task<bool> SetActivationTokenAsync(Guid userId, string token, DateTime expiry);
    Task<bool> ActivateUserAsync(string token);
    Task<bool> SetPasswordResetTokenAsync(Guid userId, string token, DateTime expiry);
    Task<bool> ResetPasswordAsync(string token, string newPassword, PasswordHasher hasher);
}

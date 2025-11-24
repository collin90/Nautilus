using Nautilus.Api.Domain.Users;

namespace Nautilus.Api.Infrastructure;

public interface IAuthRepository
{
    Task<Guid?> RegisterAsync(string userName, string email, string password);
    Task<User?> GetUserByIdentifierAsync(string identifier);
}

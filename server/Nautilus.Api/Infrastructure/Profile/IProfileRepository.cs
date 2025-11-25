using Nautilus.Api.Domain.Users;

namespace Nautilus.Api.Infrastructure.Profile;

public interface IProfileRepository
{
    Task<User?> GetUserProfileAsync(Guid userId);
}
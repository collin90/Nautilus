using Nautilus.Api.Data;

namespace Nautilus.Api.Services;

public interface IProfileService
{
    Task<User?> GetUserProfileAsync(Guid userId);
}
using Nautilus.Api.Data;

namespace Nautilus.Api.Backend;

/// <summary>
/// Backend interface for profile operations
/// </summary>
public interface IProfileBackend
{
    Task<User?> GetUserProfileAsync(Guid userId);
}

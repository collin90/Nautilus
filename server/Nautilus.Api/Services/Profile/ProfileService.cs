using Nautilus.Api.Backend;
using Nautilus.Api.Data;

namespace Nautilus.Api.Services.Profile;

/// <summary>
/// Profile service that delegates to backend implementation
/// </summary>
public class ProfileService(IProfileBackend backend, ILogger<ProfileService> logger) : IProfileService
{
    private readonly IProfileBackend _backend = backend;
    private readonly ILogger<ProfileService> _logger = logger;

    public Task<User?> GetUserProfileAsync(Guid userId)
    {
        _logger.LogDebug("ProfileService.GetUserProfileAsync called for user: {UserId}", userId);
        return _backend.GetUserProfileAsync(userId);
    }
}

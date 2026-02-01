using Nautilus.Api.Data;

namespace Nautilus.Api.Backend.Sql;

/// <summary>
/// Database implementation of profile backend (TODO: Implement)
/// </summary>
public class ProfileBackend(ILogger<ProfileBackend> logger) : IProfileBackend
{
    private readonly ILogger<ProfileBackend> _logger = logger;

    public async Task<User?> GetUserProfileAsync(Guid userId)
    {
        // TODO: Implement database query
        _logger.LogWarning("ProfileBackend.GetUserProfileAsync not implemented - using placeholder");
        await Task.Delay(100);
        return null;
    }
}

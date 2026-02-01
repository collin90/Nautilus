using Nautilus.Api.Data;

namespace Nautilus.Api.Backend.Mock;

/// <summary>
/// In-memory implementation of profile backend for local testing
/// </summary>
public class ProfileBackend(ILogger<ProfileBackend> logger) : IProfileBackend
{
    private readonly ILogger<ProfileBackend> _logger = logger;

    public Task<User?> GetUserProfileAsync(Guid userId)
    {
        _logger.LogInformation("Fetching user profile for UserId: {UserId}", userId);
        _logger.LogInformation("Current users in store count: {UserCount}", UserStore.Users.Count);
        foreach (var usr in UserStore.Users)
        {
            _logger.LogInformation("User in store: {UserId}, {UserName}", usr.UserId, usr.UserName);
        }

        var user = UserStore.Users.FirstOrDefault(u => u.UserId == userId);
        return Task.FromResult(user);
    }
}

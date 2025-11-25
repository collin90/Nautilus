using Nautilus.Api.Domain.Users;
using Nautilus.Api.Infrastructure.Shared;

namespace Nautilus.Api.Infrastructure.Profile;

public class ProfileRepositoryMem(ILogger<ProfileRepositoryMem> logger) : IProfileRepository
{

    private readonly ILogger<ProfileRepositoryMem> _logger = logger;

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

using Nautilus.Api.Data;

namespace Nautilus.Api.Backend.Mock;

/// <summary>
/// Shared in-memory user store for mock backend implementations
/// </summary>
public static class UserStore
{
    public static List<User> Users { get; } = [];
}

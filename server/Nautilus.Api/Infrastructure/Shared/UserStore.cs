using Nautilus.Api.Domain.Users;

namespace Nautilus.Api.Infrastructure.Shared;

public static class UserStore
{
    public static List<User> Users { get; } = [];
}

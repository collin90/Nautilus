using Nautilus.Api.Application.Profile.User;

namespace Nautilus.Api.Application.Profile;

public static class Profile
{
    public static IEndpointRouteBuilder MapProfileRoutes(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/profile").WithTags("Profile");

        group.MapGet("/{userId}", UserHandler.Handle);

        return routes;
    }
}
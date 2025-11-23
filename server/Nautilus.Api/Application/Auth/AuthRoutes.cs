using Nautilus.Api.Application.Auth.Login;
using Nautilus.Api.Application.Auth.Register;

namespace Nautilus.Api.Application.Auth;

public static class AuthRoutes
{
    public static IEndpointRouteBuilder MapAuthRoutes(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth").WithTags("Auth");

        group.MapPost("/register", RegisterHandler.Handle);
        group.MapPost("/login", LoginHandler.Handle);

        return routes;
    }
}

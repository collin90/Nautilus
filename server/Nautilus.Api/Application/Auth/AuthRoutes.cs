using Nautilus.Api.Application.Auth.Login;
using Nautilus.Api.Application.Auth.Register;
using Nautilus.Api.Application.Auth.PasswordReset;
using Nautilus.Api.Application.Auth.Activate;

namespace Nautilus.Api.Application.Auth;

public static class AuthRoutes
{
    public static IEndpointRouteBuilder MapAuthRoutes(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth").WithTags("Auth");

        group.MapPost("/register", RegisterHandler.Handle);
        group.MapPost("/login", LoginHandler.Handle);
        group.MapPost("/activate", ActivateAccountHandler.Handle);
        group.MapPost("/resend-activation", ResendActivationHandler.Handle);
        group.MapPost("/request-password-reset", RequestPasswordResetHandler.Handle);
        group.MapPost("/reset-password", ResetPasswordHandler.Handle);

        return routes;
    }
}

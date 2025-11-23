using Nautilus.Api.Infrastructure;

namespace Nautilus.Api.Application.Auth.Register;

public static class RegisterHandler
{
    public static async Task<IResult> Handle(
        RegisterRequest request,
        AuthRepository repo)
    {
        var userId = await repo.RegisterAsync(
            request.UserName,
            request.Email,
            request.Password
        );

        if (userId == null)
            return Results.BadRequest("User already exists.");

        return Results.Ok(new { UserId = userId });
    }
}

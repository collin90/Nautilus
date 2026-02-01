using Nautilus.Api.Services;
using Nautilus.Api.Services.Security;
using Nautilus.Api.Lib;

namespace Nautilus.Api.Application.Auth.Login;

public static class LoginHandler
{
    public static async Task<IResult> Handle(
        LoginRequest request,
        IAuthService repo,
        JwtService jwtService)
    {
        var user = await repo.GetUserByIdentifierAsync(request.Identifier);

        if (user is null)
            return Results.BadRequest("Invalid credentials. User could not be found matching this email or username.");

        // Check password using password hasher
        if (user.PasswordHash is null || user.PasswordSalt is null || !PasswordHasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt))
            return Results.BadRequest("Invalid credentials. Incorrect Password.");

        // Check if account is activated
        if (!user.IsActivated)
            return Results.Unauthorized();

        // Generate JWT token
        var token = jwtService.GenerateToken(user.UserId, user.UserName, user.Email);

        return Results.Ok(new { Token = token });
    }
}

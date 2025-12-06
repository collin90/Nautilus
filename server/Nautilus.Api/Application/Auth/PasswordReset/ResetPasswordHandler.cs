using Nautilus.Api.Infrastructure.Auth;
using Nautilus.Api.Infrastructure.Security;

namespace Nautilus.Api.Application.Auth.PasswordReset;

public static class ResetPasswordHandler
{
    public static async Task<IResult> Handle(
        ResetPasswordRequest request,
        IAuthRepository repo,
        PasswordHasher hasher,
        ILogger<ResetPasswordRequest> logger)
    {
        if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return Results.BadRequest(new { Message = "Token and new password are required." });
        }

        var success = await repo.ResetPasswordAsync(request.Token, request.NewPassword, hasher);

        if (!success)
        {
            logger.LogWarning("Password reset failed for token: {Token}", request.Token);
            return Results.BadRequest(new { Message = "Invalid or expired reset token." });
        }

        logger.LogInformation("Password reset successfully with token: {Token}", request.Token);
        return Results.Ok(new { Message = "Password reset successfully. You can now log in with your new password." });
    }
}

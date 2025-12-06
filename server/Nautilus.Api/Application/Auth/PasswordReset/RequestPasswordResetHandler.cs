using Nautilus.Api.Infrastructure.Auth;
using Nautilus.Api.Infrastructure.Email;

namespace Nautilus.Api.Application.Auth.PasswordReset;

public static class RequestPasswordResetHandler
{
    public static async Task<IResult> Handle(
        RequestPasswordResetRequest request,
        IAuthRepository repo,
        IEmailService emailService,
        ILogger<RequestPasswordResetRequest> logger)
    {
        // Get user by email
        var user = await repo.GetUserByIdentifierAsync(request.Email);

        // Always return success to prevent email enumeration
        if (user == null)
        {
            logger.LogInformation("Password reset requested for non-existent email: {Email}", request.Email);
            return Results.Ok(new { Message = "If an account with that email exists, a password reset link has been sent." });
        }

        // Generate reset token (valid for 1 hour)
        var resetToken = TokenGenerator.GenerateToken();
        var tokenExpiry = DateTime.UtcNow.AddHours(1);

        // Store reset token in repository
        await repo.SetPasswordResetTokenAsync(user.UserId, resetToken, tokenExpiry);

        // Send password reset email
        var emailSent = await emailService.SendPasswordResetEmailAsync(
            user.Email,
            user.UserName,
            resetToken
        );

        if (!emailSent)
        {
            logger.LogWarning(
                "Password reset requested for user {UserId} but email failed to send",
                user.UserId
            );
        }

        return Results.Ok(new { Message = "If an account with that email exists, a password reset link has been sent." });
    }
}

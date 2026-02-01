using Nautilus.Api.Services;
using Nautilus.Api.Lib;

namespace Nautilus.Api.Application.Auth.Activate;

public static class ResendActivationHandler
{
    public static async Task<IResult> Handle(
        ResendActivationRequest request,
        IAuthService repo,
        IEmailService emailService,
        ILogger<ResendActivationRequest> logger)
    {
        // Look up user by email
        var user = await repo.GetUserByIdentifierAsync(request.Email);

        // Always return success to prevent email enumeration
        if (user == null)
        {
            logger.LogInformation("Resend activation requested for non-existent email: {Email}", request.Email);
            return Results.Ok(new { Message = "If an unactivated account exists with that email, a new activation link has been sent." });
        }

        // Check if already activated
        if (user.IsActivated)
        {
            logger.LogInformation("Resend activation requested for already activated user: {Email}", request.Email);
            return Results.Ok(new { Message = "If an unactivated account exists with that email, a new activation link has been sent." });
        }

        // Generate new activation token (valid for 24 hours)
        var activationToken = TokenGenerator.GenerateToken();
        var tokenExpiry = DateTime.UtcNow.AddHours(24);

        // Store new activation token
        await repo.SetActivationTokenAsync(user.UserId, activationToken, tokenExpiry);

        // Send activation email
        var emailSent = await emailService.SendActivationEmailAsync(
            request.Email,
            user.UserName,
            activationToken
        );

        if (!emailSent)
        {
            logger.LogWarning(
                "Failed to resend activation email to {Email}",
                request.Email
            );
        }

        return Results.Ok(new { Message = "If an unactivated account exists with that email, a new activation link has been sent." });
    }
}

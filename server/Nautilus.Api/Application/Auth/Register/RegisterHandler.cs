using Nautilus.Api.Infrastructure.Auth;
using Nautilus.Api.Infrastructure.Email;

namespace Nautilus.Api.Application.Auth.Register;

public static class RegisterHandler
{
    public static async Task<IResult> Handle(
        RegisterRequest request,
        IAuthRepository repo,
        IEmailService emailService,
        ILogger<RegisterRequest> logger)
    {
        var userId = await repo.RegisterAsync(
            request.UserName,
            request.Email,
            request.Password
        );

        if (userId == null)
            return Results.BadRequest("User already exists.");

        // Generate activation token (valid for 24 hours)
        var activationToken = TokenGenerator.GenerateToken();
        var tokenExpiry = DateTime.UtcNow.AddHours(24);

        // Store activation token in repository
        await repo.SetActivationTokenAsync(userId.Value, activationToken, tokenExpiry);

        // Send activation email
        var emailSent = await emailService.SendActivationEmailAsync(
            request.Email,
            request.UserName,
            activationToken
        );

        if (!emailSent)
        {
            logger.LogWarning(
                "User {UserId} registered but activation email failed to send to {Email}",
                userId,
                request.Email
            );
        }

        return Results.Ok(new { UserId = userId, Message = "Registration successful. Please check your email to activate your account." });
    }
}

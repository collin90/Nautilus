using Nautilus.Api.Infrastructure.Auth;

namespace Nautilus.Api.Application.Auth.Activate;

public static class ActivateAccountHandler
{
    public static async Task<IResult> Handle(
        ActivateAccountRequest request,
        IAuthRepository repo,
        ILogger<ActivateAccountRequest> logger)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            logger.LogWarning("Account activation attempted with empty token.");
            return Results.BadRequest(new { Message = "Invalid activation token." });
        }

        var success = await repo.ActivateUserAsync(request.Token);

        if (!success)
        {
            logger.LogWarning("Account activation failed for token: {Token}", request.Token);
            return Results.BadRequest(new { Message = "Invalid or expired activation token." });
        }

        logger.LogInformation("Account activated successfully with token: {Token}", request.Token);
        return Results.Ok(new { Message = "Account activated successfully. You can now log in." });
    }
}

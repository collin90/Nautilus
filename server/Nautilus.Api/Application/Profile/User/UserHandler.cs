
using Nautilus.Api.Services;
namespace Nautilus.Api.Application.Profile.User;

public static class UserHandler
{
    public static async Task<IResult> Handle(HttpContext context, Guid userId, IProfileService repo)
    {
        try
        {
            var user = await repo.GetUserProfileAsync(userId);
            if (user == null)
                return Results.NotFound("User not found.");

            return Results.Ok(new
            {
                user.UserName,
                user.Email
            });
        }
        catch (Exception)
        {
            return Results.Problem("An error occurred while retrieving the user profile.");
        }
    }
}
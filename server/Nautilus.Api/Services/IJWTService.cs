using System.Security.Claims;

namespace Nautilus.Api.Services;

public interface IJWTService
{
    string GenerateToken(Guid userId, string userName, string email);
    public ClaimsPrincipal? ValidateToken(string token);
    public Guid? GetUserIdFromToken(string token);

}
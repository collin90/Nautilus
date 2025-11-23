namespace Nautilus.Api.Application.Auth.Register;

public record RegisterRequest(
    string UserName,
    string Email,
    string Password
);

namespace Nautilus.Api.Application.Auth.Login;

public record LoginRequest(
    string Identifier,
    string Password
);

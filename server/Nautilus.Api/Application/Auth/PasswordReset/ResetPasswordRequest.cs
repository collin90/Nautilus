namespace Nautilus.Api.Application.Auth.PasswordReset;

public record ResetPasswordRequest(string Token, string NewPassword);

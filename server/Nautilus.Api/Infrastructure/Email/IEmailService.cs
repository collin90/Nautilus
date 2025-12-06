namespace Nautilus.Api.Infrastructure.Email;

public interface IEmailService
{
    Task<bool> SendActivationEmailAsync(string email, string name, string activationToken);
    Task<bool> SendPasswordResetEmailAsync(string email, string name, string resetToken);
}

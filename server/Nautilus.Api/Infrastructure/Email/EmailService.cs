using System.Net.Http.Json;

namespace Nautilus.Api.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmailService> _logger;
    private readonly string _emailServiceUrl;
    private readonly string _frontendUrl;

    public EmailService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<EmailService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _emailServiceUrl = configuration["EmailService:BaseUrl"] ?? "http://localhost:3001";
        _frontendUrl = configuration["EmailService:FrontendUrl"] ?? "http://localhost:5173";
    }

    public async Task<bool> SendActivationEmailAsync(string email, string name, string activationToken)
    {
        try
        {
            var activationUrl = $"{_frontendUrl}/activate?token={activationToken}";

            var request = new
            {
                email,
                name,
                activationUrl
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{_emailServiceUrl}/api/send/activate",
                request
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Failed to send activation email to {Email}: {StatusCode} - {Error}",
                    email,
                    response.StatusCode,
                    error
                );
                return false;
            }

            _logger.LogInformation("Activation email sent successfully to {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending activation email to {Email}", email);
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(string email, string name, string resetToken)
    {
        try
        {
            var resetUrl = $"{_frontendUrl}/reset-password?token={resetToken}";

            var request = new
            {
                email,
                name,
                resetUrl
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"{_emailServiceUrl}/api/send/reset-password",
                request
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Failed to send password reset email to {Email}: {StatusCode} - {Error}",
                    email,
                    response.StatusCode,
                    error
                );
                return false;
            }

            _logger.LogInformation("Password reset email sent successfully to {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception sending password reset email to {Email}", email);
            return false;
        }
    }
}

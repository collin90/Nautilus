namespace Nautilus.Api.Domain.Users;

public class User
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public byte[] PasswordHash { get; set; } = default!;
    public byte[] PasswordSalt { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActivated { get; set; }
    public string? ActivationToken { get; set; }
    public DateTime? ActivationTokenExpiry { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }
}

using BusTicket.Domain.Common;
using BusTicket.Domain.Enums;

namespace BusTicket.Domain.Entities;

/// <summary>
/// Aggregate Root: AdminUser - represents system administrators
/// </summary>
public class AdminUser : Entity, IAggregateRoot
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public AdminRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastLoginDate { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    private AdminUser() { } // EF Core constructor

    private AdminUser(
        string email,
        string passwordHash,
        string fullName,
        AdminRole role)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Role = role;
        IsActive = true;
    }

    public static AdminUser Create(
        string email,
        string passwordHash,
        string fullName,
        AdminRole role = AdminRole.Admin)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));

        // Basic email validation
        if (!email.Contains('@'))
            throw new ArgumentException("Invalid email format", nameof(email));

        return new AdminUser(email.ToLower().Trim(), passwordHash, fullName.Trim(), role);
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));

        FullName = fullName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(AdminRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin()
    {
        LastLoginDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token cannot be empty", nameof(refreshToken));

        if (expiryTime <= DateTime.UtcNow)
            throw new ArgumentException("Refresh token expiry time must be in the future", nameof(expiryTime));

        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsRefreshTokenValid(string token)
    {
        if (string.IsNullOrWhiteSpace(RefreshToken))
            return false;

        if (RefreshTokenExpiryTime == null || RefreshTokenExpiryTime <= DateTime.UtcNow)
            return false;

        return RefreshToken == token;
    }
}

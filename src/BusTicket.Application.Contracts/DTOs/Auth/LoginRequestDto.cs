namespace BusTicket.Application.Contracts.DTOs.Auth;

/// <summary>
/// DTO for admin login request
/// </summary>
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

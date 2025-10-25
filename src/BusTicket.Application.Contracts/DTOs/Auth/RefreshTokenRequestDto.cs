namespace BusTicket.Application.Contracts.DTOs.Auth;

/// <summary>
/// DTO for refresh token request
/// </summary>
public class RefreshTokenRequestDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

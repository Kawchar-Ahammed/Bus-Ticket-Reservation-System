namespace BusTicket.Application.Contracts.DTOs.Auth;

/// <summary>
/// DTO for token response (refresh token scenario)
/// </summary>
public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

using BusTicket.Application.Contracts.DTOs.Auth;
using MediatR;

namespace BusTicket.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Command to refresh an access token
/// </summary>
public class RefreshTokenCommand : IRequest<TokenResponseDto>
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

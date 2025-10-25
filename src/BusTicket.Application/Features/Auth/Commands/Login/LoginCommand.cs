using BusTicket.Application.Contracts.DTOs.Auth;
using MediatR;

namespace BusTicket.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command to authenticate an admin user
/// </summary>
public class LoginCommand : IRequest<LoginResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

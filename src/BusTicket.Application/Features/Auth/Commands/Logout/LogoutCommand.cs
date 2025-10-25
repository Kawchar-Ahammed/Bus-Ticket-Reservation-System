using MediatR;

namespace BusTicket.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Command to logout an admin user
/// </summary>
public class LogoutCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
}

using BusTicket.Application.Contracts.DTOs;
using MediatR;

namespace BusTicket.Application.Queries;

/// <summary>
/// Query to get booking detail by ID for admin
/// </summary>
public class GetBookingByIdForAdminQuery : IRequest<AdminBookingDetailDto?>
{
    public Guid TicketId { get; set; }

    public GetBookingByIdForAdminQuery(Guid ticketId)
    {
        TicketId = ticketId;
    }
}

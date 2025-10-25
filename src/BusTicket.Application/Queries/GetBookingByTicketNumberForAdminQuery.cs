using BusTicket.Application.Contracts.DTOs;
using MediatR;

namespace BusTicket.Application.Queries;

/// <summary>
/// Query to get booking by ticket number for admin
/// </summary>
public class GetBookingByTicketNumberForAdminQuery : IRequest<AdminBookingDetailDto?>
{
    public string TicketNumber { get; set; } = string.Empty;

    public GetBookingByTicketNumberForAdminQuery(string ticketNumber)
    {
        TicketNumber = ticketNumber;
    }
}

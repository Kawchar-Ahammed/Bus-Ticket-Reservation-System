using BusTicket.Application.Contracts.DTOs;
using MediatR;

namespace BusTicket.Application.Queries;

/// <summary>
/// Query to get booking details by ticket number
/// </summary>
public class GetBookingByTicketNumberQuery : IRequest<BookingDetailDto?>
{
    public string TicketNumber { get; set; } = string.Empty;

    public GetBookingByTicketNumberQuery(string ticketNumber)
    {
        TicketNumber = ticketNumber;
    }
}

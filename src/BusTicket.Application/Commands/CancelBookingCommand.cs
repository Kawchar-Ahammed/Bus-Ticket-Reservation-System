using BusTicket.Application.Contracts.DTOs;
using MediatR;

namespace BusTicket.Application.Commands;

/// <summary>
/// Command to cancel a booking
/// </summary>
public class CancelBookingCommand : IRequest<CancelBookingResultDto>
{
    public string TicketNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string CancellationReason { get; set; } = string.Empty;

    public CancelBookingCommand(string ticketNumber, string phoneNumber, string cancellationReason = "")
    {
        TicketNumber = ticketNumber;
        PhoneNumber = phoneNumber;
        CancellationReason = cancellationReason;
    }
}

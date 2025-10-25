using BusTicket.Application.Contracts.DTOs;
using MediatR;

namespace BusTicket.Application.Queries;

/// <summary>
/// Query to get all bookings for a phone number
/// </summary>
public class GetBookingsByPhoneNumberQuery : IRequest<List<BookingHistoryDto>>
{
    public string PhoneNumber { get; set; } = string.Empty;

    public GetBookingsByPhoneNumberQuery(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
    }
}

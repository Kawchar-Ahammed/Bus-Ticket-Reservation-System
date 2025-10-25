using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicket.Application.Handlers.Queries;

/// <summary>
/// Handler for GetBookingsByPhoneNumberQuery
/// </summary>
public class GetBookingsByPhoneNumberQueryHandler 
    : IRequestHandler<GetBookingsByPhoneNumberQuery, List<BookingHistoryDto>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ILogger<GetBookingsByPhoneNumberQueryHandler> _logger;

    public GetBookingsByPhoneNumberQueryHandler(
        ITicketRepository ticketRepository,
        ILogger<GetBookingsByPhoneNumberQueryHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _logger = logger;
    }

    public async Task<List<BookingHistoryDto>> Handle(
        GetBookingsByPhoneNumberQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting bookings for phone number: {PhoneNumber}", request.PhoneNumber);

        var tickets = await _ticketRepository.GetTicketsByPhoneNumberWithDetailsAsync(
            request.PhoneNumber,
            cancellationToken);

        if (!tickets.Any())
        {
            _logger.LogInformation("No bookings found for phone number: {PhoneNumber}", request.PhoneNumber);
            return new List<BookingHistoryDto>();
        }

        var bookings = tickets.Select(ticket => new BookingHistoryDto
        {
            TicketId = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            PassengerId = ticket.PassengerId,
            PassengerName = ticket.Passenger.Name,
            PhoneNumber = ticket.Passenger.PhoneNumber.Value,
            Email = ticket.Passenger.Email ?? string.Empty,
            
            // Bus Schedule Details
            BusScheduleId = ticket.BusScheduleId,
            CompanyName = ticket.BusSchedule.Bus.Company.Name,
            BusName = ticket.BusSchedule.Bus.BusName,
            FromCity = ticket.BusSchedule.Route.FromLocation.City,
            ToCity = ticket.BusSchedule.Route.ToLocation.City,
            JourneyDate = ticket.BusSchedule.JourneyDate,
            DepartureTime = ticket.BusSchedule.DepartureTime,
            
            // Seat Details
            SeatNumber = ticket.Seat.SeatNumber,
            BoardingPoint = ticket.BoardingPoint?.ToString() ?? string.Empty,
            DroppingPoint = ticket.DroppingPoint?.ToString() ?? string.Empty,
            
            // Booking Details
            BookingDate = ticket.BookingDate,
            Fare = ticket.Fare.Amount,
            Currency = ticket.Fare.Currency,
            BookingStatus = ticket.IsCancelled ? "Cancelled" : (ticket.IsConfirmed ? "Confirmed" : "Pending"),
            PaymentStatus = ticket.IsConfirmed ? "Paid" : "Pending"
        })
        .ToList();

        _logger.LogInformation("Found {Count} bookings for phone number: {PhoneNumber}", 
            bookings.Count, request.PhoneNumber);

        return bookings;
    }
}

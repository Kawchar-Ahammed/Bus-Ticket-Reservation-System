using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicket.Application.Handlers.Queries;

/// <summary>
/// Handler for GetBookingByTicketNumberQuery
/// </summary>
public class GetBookingByTicketNumberQueryHandler 
    : IRequestHandler<GetBookingByTicketNumberQuery, BookingDetailDto?>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ILogger<GetBookingByTicketNumberQueryHandler> _logger;

    public GetBookingByTicketNumberQueryHandler(
        ITicketRepository ticketRepository,
        ILogger<GetBookingByTicketNumberQueryHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _logger = logger;
    }

    public async Task<BookingDetailDto?> Handle(
        GetBookingByTicketNumberQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting booking details for ticket: {TicketNumber}", request.TicketNumber);

        var ticket = await _ticketRepository.GetByTicketNumberWithDetailsAsync(
            request.TicketNumber,
            cancellationToken);

        if (ticket == null)
        {
            _logger.LogWarning("Ticket not found: {TicketNumber}", request.TicketNumber);
            return null;
        }

        var bookingDetail = new BookingDetailDto
        {
            TicketId = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            
            // Passenger Info
            PassengerId = ticket.PassengerId,
            PassengerName = ticket.Passenger.Name,
            PhoneNumber = ticket.Passenger.PhoneNumber.Value,
            Email = ticket.Passenger.Email ?? string.Empty,
            Gender = ticket.Passenger.Gender?.ToString(),
            Age = ticket.Passenger.Age,
            
            // Bus Schedule Details
            BusScheduleId = ticket.BusScheduleId,
            CompanyName = ticket.BusSchedule.Bus.Company.Name,
            BusName = ticket.BusSchedule.Bus.BusName,
            BusNumber = ticket.BusSchedule.Bus.BusNumber,
            FromCity = ticket.BusSchedule.Route.FromLocation.City,
            ToCity = ticket.BusSchedule.Route.ToLocation.City,
            JourneyDate = ticket.BusSchedule.JourneyDate,
            DepartureTime = ticket.BusSchedule.DepartureTime,
            
            // Seat Details
            SeatId = ticket.SeatId,
            SeatNumber = ticket.Seat.SeatNumber,
            BoardingPoint = ticket.BoardingPoint?.ToString() ?? string.Empty,
            DroppingPoint = ticket.DroppingPoint?.ToString() ?? string.Empty,
            
            // Booking Details
            BookingDate = ticket.BookingDate,
            Fare = ticket.Fare.Amount,
            Currency = ticket.Fare.Currency,
            BookingStatus = ticket.IsCancelled ? "Cancelled" : (ticket.IsConfirmed ? "Confirmed" : "Pending"),
            PaymentStatus = ticket.IsConfirmed ? "Paid" : "Pending",
            
            // Timestamps
            CreatedAt = ticket.CreatedAt,
            CancelledAt = ticket.CancellationDate
        };

        _logger.LogInformation("Successfully retrieved booking details for ticket: {TicketNumber}", 
            request.TicketNumber);

        return bookingDetail;
    }
}

using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using BusTicket.Domain.Exceptions;

namespace BusTicket.Domain.Services;

/// <summary>
/// Domain Service: Handles seat booking business logic
/// Implements complex business rules that don't naturally fit in entities
/// </summary>
public interface ISeatBookingDomainService
{
    /// <summary>
    /// Books a seat with transaction safety
    /// </summary>
    Task<Ticket> BookSeatAsync(Seat seat, Passenger passenger, BusSchedule busSchedule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if seat can be booked
    /// </summary>
    bool CanBookSeat(Seat seat);

    /// <summary>
    /// Confirms a booking (marks seat as Sold)
    /// </summary>
    void ConfirmBooking(Ticket ticket, Seat seat);

    /// <summary>
    /// Cancels a booking and releases the seat
    /// </summary>
    void CancelBooking(Ticket ticket, Seat seat);
}

/// <summary>
/// Implementation of Seat Booking Domain Service
/// </summary>
public class SeatBookingDomainService : ISeatBookingDomainService
{
    public Task<Ticket> BookSeatAsync(
        Seat seat,
        Passenger passenger,
        BusSchedule busSchedule,
        CancellationToken cancellationToken = default)
    {
        if (seat == null)
            throw new ArgumentNullException(nameof(seat));

        if (passenger == null)
            throw new ArgumentNullException(nameof(passenger));

        if (busSchedule == null)
            throw new ArgumentNullException(nameof(busSchedule));

        // Validate seat availability
        if (!CanBookSeat(seat))
            throw new SeatNotAvailableException(seat.SeatNumber);

        // Validate schedule is active and in future
        if (!busSchedule.IsActive)
            throw new BusinessRuleViolationException("Bus schedule is not active");

        if (busSchedule.JourneyDate < DateTime.UtcNow.Date)
            throw new BusinessRuleViolationException("Cannot book seats for past journeys");

        // Create ticket
        var ticketNumber = Ticket.GenerateTicketNumber();
        var ticket = Ticket.Create(
            ticketNumber,
            busSchedule.Id,
            passenger.Id,
            seat.Id,
            busSchedule.BoardingPoint,
            busSchedule.DroppingPoint,
            busSchedule.Fare);

        // Book the seat (domain logic)
        seat.Book(ticket.Id);

        return Task.FromResult(ticket);
    }

    public bool CanBookSeat(Seat seat)
    {
        if (seat == null)
            return false;

        return seat.Status == SeatStatus.Available;
    }

    public void ConfirmBooking(Ticket ticket, Seat seat)
    {
        if (ticket == null)
            throw new ArgumentNullException(nameof(ticket));

        if (seat == null)
            throw new ArgumentNullException(nameof(seat));

        if (ticket.SeatId != seat.Id)
            throw new BusinessRuleViolationException("Ticket and seat do not match");

        if (ticket.IsCancelled)
            throw new BusinessRuleViolationException("Cannot confirm a cancelled ticket");

        // Confirm ticket
        ticket.Confirm();

        // Mark seat as sold
        seat.ConfirmBooking();
    }

    public void CancelBooking(Ticket ticket, Seat seat)
    {
        if (ticket == null)
            throw new ArgumentNullException(nameof(ticket));

        if (seat == null)
            throw new ArgumentNullException(nameof(seat));

        if (ticket.SeatId != seat.Id)
            throw new BusinessRuleViolationException("Ticket and seat do not match");

        // Cancel ticket
        ticket.Cancel();

        // Release seat
        seat.CancelBooking();
    }
}

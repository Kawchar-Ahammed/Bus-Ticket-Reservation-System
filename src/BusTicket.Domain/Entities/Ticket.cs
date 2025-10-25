using BusTicket.Domain.Common;
using BusTicket.Domain.ValueObjects;

namespace BusTicket.Domain.Entities;

/// <summary>
/// Aggregate Root: Ticket - represents a booking
/// </summary>
public class Ticket : Entity, IAggregateRoot
{
    public string TicketNumber { get; private set; }
    public Guid BusScheduleId { get; private set; }
    public Guid PassengerId { get; private set; }
    public Guid SeatId { get; private set; }
    public Address BoardingPoint { get; private set; } = null!;
    public Address DroppingPoint { get; private set; } = null!;
    public Money Fare { get; private set; } = null!;
    public DateTime BookingDate { get; private set; }
    public bool IsConfirmed { get; private set; }
    public bool IsCancelled { get; private set; }
    public DateTime? ConfirmationDate { get; private set; }
    public DateTime? CancellationDate { get; private set; }
    public string? CancellationReason { get; private set; }

    // Navigation properties
    public virtual BusSchedule BusSchedule { get; private set; } = null!;
    public virtual Passenger Passenger { get; private set; } = null!;
    public virtual Seat Seat { get; private set; } = null!;

    private Ticket() { } // EF Core constructor

    private Ticket(
        string ticketNumber,
        Guid busScheduleId,
        Guid passengerId,
        Guid seatId,
        Address boardingPoint,
        Address droppingPoint,
        Money fare)
    {
        TicketNumber = ticketNumber;
        BusScheduleId = busScheduleId;
        PassengerId = passengerId;
        SeatId = seatId;
        BoardingPoint = boardingPoint;
        DroppingPoint = droppingPoint;
        Fare = fare;
        BookingDate = DateTime.UtcNow;
        IsConfirmed = false;
        IsCancelled = false;
    }

    public static Ticket Create(
        string ticketNumber,
        Guid busScheduleId,
        Guid passengerId,
        Guid seatId,
        Address boardingPoint,
        Address droppingPoint,
        Money fare)
    {
        if (string.IsNullOrWhiteSpace(ticketNumber))
            throw new ArgumentException("Ticket number cannot be empty", nameof(ticketNumber));

        if (busScheduleId == Guid.Empty)
            throw new ArgumentException("Bus schedule ID cannot be empty", nameof(busScheduleId));

        if (passengerId == Guid.Empty)
            throw new ArgumentException("Passenger ID cannot be empty", nameof(passengerId));

        if (seatId == Guid.Empty)
            throw new ArgumentException("Seat ID cannot be empty", nameof(seatId));

        if (boardingPoint == null)
            throw new ArgumentNullException(nameof(boardingPoint));

        if (droppingPoint == null)
            throw new ArgumentNullException(nameof(droppingPoint));

        if (fare == null)
            throw new ArgumentNullException(nameof(fare));

        return new Ticket(ticketNumber, busScheduleId, passengerId, seatId, boardingPoint, droppingPoint, fare);
    }

    /// <summary>
    /// Confirms the ticket (after payment)
    /// </summary>
    public void Confirm()
    {
        if (IsConfirmed)
            throw new InvalidOperationException($"Ticket {TicketNumber} is already confirmed");

        if (IsCancelled)
            throw new InvalidOperationException($"Ticket {TicketNumber} is cancelled and cannot be confirmed");

        IsConfirmed = true;
        ConfirmationDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the ticket
    /// </summary>
    public void Cancel(string? reason = null)
    {
        if (IsCancelled)
            throw new InvalidOperationException($"Ticket {TicketNumber} is already cancelled");

        IsCancelled = true;
        CancellationDate = DateTime.UtcNow;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Generates a unique ticket number
    /// </summary>
    public static string GenerateTicketNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"TKT-{timestamp}-{random}";
    }
}

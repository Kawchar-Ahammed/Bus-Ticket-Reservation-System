using BusTicket.Domain.Common;
using BusTicket.Domain.Enums;

namespace BusTicket.Domain.Entities;

/// <summary>
/// Entity: Seat - represents a specific seat in a bus schedule
/// </summary>
public class Seat : Entity
{
    public Guid BusScheduleId { get; private set; }
    public string SeatNumber { get; private set; }
    public int Row { get; private set; }
    public int Column { get; private set; }
    public SeatStatus Status { get; private set; }
    public Guid? TicketId { get; private set; }

    // Navigation properties
    public virtual BusSchedule BusSchedule { get; private set; } = null!;
    public virtual Ticket? Ticket { get; private set; }

    private Seat() { } // EF Core constructor

    private Seat(Guid busScheduleId, string seatNumber, int row, int column)
    {
        BusScheduleId = busScheduleId;
        SeatNumber = seatNumber;
        Row = row;
        Column = column;
        Status = SeatStatus.Available;
    }

    public static Seat Create(Guid busScheduleId, string seatNumber, int row, int column)
    {
        if (busScheduleId == Guid.Empty)
            throw new ArgumentException("Bus schedule ID cannot be empty", nameof(busScheduleId));

        if (string.IsNullOrWhiteSpace(seatNumber))
            throw new ArgumentException("Seat number cannot be empty", nameof(seatNumber));

        if (row <= 0)
            throw new ArgumentException("Row must be greater than zero", nameof(row));

        if (column <= 0)
            throw new ArgumentException("Column must be greater than zero", nameof(column));

        return new Seat(busScheduleId, seatNumber, row, column);
    }

    /// <summary>
    /// Books the seat (domain logic)
    /// </summary>
    public void Book(Guid ticketId)
    {
        if (Status != SeatStatus.Available)
            throw new InvalidOperationException($"Seat {SeatNumber} is not available for booking. Current status: {Status}");

        Status = SeatStatus.Booked;
        TicketId = ticketId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Confirms the booking (marks as Sold)
    /// </summary>
    public void ConfirmBooking()
    {
        if (Status != SeatStatus.Booked)
            throw new InvalidOperationException($"Seat {SeatNumber} is not in booked status. Current status: {Status}");

        Status = SeatStatus.Sold;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the booking and makes seat available again
    /// </summary>
    public void CancelBooking()
    {
        if (Status != SeatStatus.Booked && Status != SeatStatus.Sold)
            throw new InvalidOperationException($"Seat {SeatNumber} cannot be cancelled. Current status: {Status}");

        Status = SeatStatus.Available;
        TicketId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Blocks the seat (admin action)
    /// </summary>
    public void Block()
    {
        if (Status == SeatStatus.Booked || Status == SeatStatus.Sold)
            throw new InvalidOperationException($"Cannot block seat {SeatNumber} that is already booked or sold");

        Status = SeatStatus.Blocked;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Unblocks the seat
    /// </summary>
    public void Unblock()
    {
        if (Status != SeatStatus.Blocked)
            throw new InvalidOperationException($"Seat {SeatNumber} is not blocked");

        Status = SeatStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsAvailable() => Status == SeatStatus.Available;
    public bool IsBooked() => Status == SeatStatus.Booked;
    public bool IsSold() => Status == SeatStatus.Sold;
    public bool IsBlocked() => Status == SeatStatus.Blocked;
}

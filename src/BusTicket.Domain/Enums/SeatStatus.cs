namespace BusTicket.Domain.Enums;

/// <summary>
/// Represents the status of a seat
/// </summary>
public enum SeatStatus
{
    /// <summary>
    /// Seat is available for booking
    /// </summary>
    Available = 1,

    /// <summary>
    /// Seat is temporarily booked (reserved)
    /// </summary>
    Booked = 2,

    /// <summary>
    /// Seat is sold (confirmed booking with payment)
    /// </summary>
    Sold = 3,

    /// <summary>
    /// Seat is blocked (maintenance or reserved by admin)
    /// </summary>
    Blocked = 4
}

namespace BusTicket.Application.Contracts.DTOs;

/// <summary>
/// DTO for seat information in seat plan
/// </summary>
public class SeatDto
{
    public Guid SeatId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Column { get; set; }
    public string Status { get; set; } = string.Empty; // Available, Booked, Sold, Blocked
}

/// <summary>
/// DTO for complete seat plan of a bus
/// </summary>
public class SeatPlanDto
{
    public Guid BusScheduleId { get; set; }
    public string BusName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public DateTime JourneyDate { get; set; }
    public TimeSpan DepartureTime { get; set; }
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public decimal Fare { get; set; }
    public string Currency { get; set; } = "BDT";
    public List<SeatDto> Seats { get; set; } = new();
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public int BookedSeats { get; set; }
}

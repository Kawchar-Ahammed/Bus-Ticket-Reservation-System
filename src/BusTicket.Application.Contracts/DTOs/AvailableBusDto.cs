namespace BusTicket.Application.Contracts.DTOs;

/// <summary>
/// DTO for available bus in search results
/// </summary>
public class AvailableBusDto
{
    public Guid BusScheduleId { get; set; }
    public Guid BusId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string BusNumber { get; set; } = string.Empty;
    public DateTime JourneyDate { get; set; }
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int BookedSeats { get; set; }
    public int SeatsLeft { get; set; }
    public decimal Fare { get; set; }
    public string Currency { get; set; } = "BDT";
    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
}

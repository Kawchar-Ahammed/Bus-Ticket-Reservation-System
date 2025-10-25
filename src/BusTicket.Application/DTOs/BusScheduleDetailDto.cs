namespace BusTicket.Application.DTOs;

public class BusScheduleDetailDto
{
    public Guid Id { get; set; }
    public Guid BusId { get; set; }
    public string BusNumber { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public Guid RouteId { get; set; }
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public decimal RouteDistance { get; set; }
    public DateTime JourneyDate { get; set; }
    public TimeSpan DepartureTime { get; set; }
    public TimeSpan ArrivalTime { get; set; }
    public decimal FareAmount { get; set; }
    public string FareCurrency { get; set; } = string.Empty;
    public string BoardingCity { get; set; } = string.Empty;
    public string DroppingCity { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int AvailableSeats { get; set; }
    public int TotalSeats { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

namespace BusTicket.Application.DTOs;

public class RouteDto
{
    public Guid Id { get; set; }
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public decimal DistanceInKm { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

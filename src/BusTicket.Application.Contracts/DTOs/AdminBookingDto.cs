namespace BusTicket.Application.Contracts.DTOs;

/// <summary>
/// DTO for admin booking list view
/// </summary>
public class AdminBookingDto
{
    public Guid TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    
    // Passenger Info
    public Guid PassengerId { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // Bus & Company
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string BusNumber { get; set; } = string.Empty;
    
    // Route
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    
    // Journey Details
    public DateTime JourneyDate { get; set; }
    public TimeSpan DepartureTime { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    
    // Booking Info
    public DateTime BookingDate { get; set; }
    public decimal Fare { get; set; }
    public string Currency { get; set; } = "BDT";
    public string BookingStatus { get; set; } = string.Empty; // Confirmed, Cancelled, Pending
    public bool IsConfirmed { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? ConfirmationDate { get; set; }
}

/// <summary>
/// DTO for admin booking detail view
/// </summary>
public class AdminBookingDetailDto : AdminBookingDto
{
    // Additional Passenger Info
    public string? Gender { get; set; }
    public int? Age { get; set; }
    
    // Additional Journey Info
    public TimeSpan ArrivalTime { get; set; }
    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
    
    // Additional Booking Info
    public Guid BusScheduleId { get; set; }
    public Guid SeatId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
}

namespace BusTicket.Application.Contracts.DTOs;

/// <summary>
/// DTO for booking history item
/// </summary>
public class BookingHistoryDto
{
    public Guid TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public Guid PassengerId { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // Bus Schedule Details
    public Guid BusScheduleId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public DateTime JourneyDate { get; set; }
    public TimeSpan DepartureTime { get; set; }
    
    // Seat Details
    public string SeatNumber { get; set; } = string.Empty;
    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
    
    // Booking Details
    public DateTime BookingDate { get; set; }
    public decimal Fare { get; set; }
    public string Currency { get; set; } = "BDT";
    public string BookingStatus { get; set; } = string.Empty; // Confirmed, Cancelled, Completed
    public string PaymentStatus { get; set; } = string.Empty; // Pending, Paid, Refunded
}

/// <summary>
/// DTO for detailed booking information
/// </summary>
public class BookingDetailDto
{
    public Guid TicketId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    
    // Passenger Info
    public Guid PassengerId { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public int? Age { get; set; }
    
    // Bus Schedule Details
    public Guid BusScheduleId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string BusNumber { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public DateTime JourneyDate { get; set; }
    public TimeSpan DepartureTime { get; set; }
    
    // Seat Details
    public Guid SeatId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
    
    // Booking Details
    public DateTime BookingDate { get; set; }
    public decimal Fare { get; set; }
    public string Currency { get; set; } = "BDT";
    public string BookingStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}

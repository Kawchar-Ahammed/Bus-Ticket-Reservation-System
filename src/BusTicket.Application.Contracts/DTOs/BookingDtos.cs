using System.ComponentModel.DataAnnotations;

namespace BusTicket.Application.Contracts.DTOs;

/// <summary>
/// Input DTO for booking a seat
/// </summary>
public class BookSeatInputDto
{
    [Required]
    public Guid BusScheduleId { get; set; }

    [Required]
    public Guid SeatId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string PassengerName { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20, MinimumLength = 10)]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required]
    [StringLength(200)]
    public string BoardingPoint { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string DroppingPoint { get; set; } = string.Empty;

    public string? Gender { get; set; } // "Male", "Female", "Other"

    [Range(1, 120)]
    public int? Age { get; set; }
}

/// <summary>
/// Result DTO after booking a seat
/// </summary>
public class BookSeatResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? TicketId { get; set; }
    public string? TicketNumber { get; set; }
    public Guid? PassengerId { get; set; }
    public string? PassengerName { get; set; }
    public string? SeatNumber { get; set; }
    public decimal? Fare { get; set; }
    public string? Currency { get; set; }
    public DateTime? BookingDate { get; set; }
}

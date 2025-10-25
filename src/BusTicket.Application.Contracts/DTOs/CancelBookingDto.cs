namespace BusTicket.Application.Contracts.DTOs;

/// <summary>
/// DTO for cancel booking result
/// </summary>
public class CancelBookingResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TicketNumber { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public string RefundStatus { get; set; } = string.Empty;
    public DateTime? CancellationDate { get; set; }
}

/// <summary>
/// DTO for cancel booking request
/// </summary>
public class CancelBookingRequestDto
{
    public string TicketNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string CancellationReason { get; set; } = string.Empty;
}

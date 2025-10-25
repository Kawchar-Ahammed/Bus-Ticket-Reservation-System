namespace BusTicket.Application.Contracts.DTOs;

public class EmailDto
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public List<string>? Cc { get; set; }
    public List<string>? Bcc { get; set; }
}

public class SmsDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class BookingNotificationDto
{
    public string TicketNumber { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string BusName { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public DateTime JourneyDate { get; set; }
    public string DepartureTime { get; set; } = string.Empty;
    public string SeatNumber { get; set; } = string.Empty;
    public string BoardingPoint { get; set; } = string.Empty;
    public string DroppingPoint { get; set; } = string.Empty;
    public decimal Fare { get; set; }
    public string Currency { get; set; } = "BDT";
}

public class CancellationNotificationDto
{
    public string TicketNumber { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public DateTime JourneyDate { get; set; }
    public decimal RefundAmount { get; set; }
    public string Currency { get; set; } = "BDT";
    public string CancellationReason { get; set; } = string.Empty;
}

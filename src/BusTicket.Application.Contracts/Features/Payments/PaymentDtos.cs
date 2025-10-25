using BusTicket.Domain.Enums;

namespace BusTicket.Application.Contracts.Features.Payments;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public string TicketNumber { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BDT";
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentMethodName { get; set; } = null!;
    public PaymentStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public PaymentGateway Gateway { get; set; }
    public string GatewayName { get; set; } = null!;
    public string? TransactionId { get; set; }
    public string? GatewayTransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public decimal RefundAmount { get; set; }
    public DateTime? RefundDate { get; set; }
    public string? RefundReason { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PaymentDetailsDto : PaymentDto
{
    public string PassengerName { get; set; } = null!;
    public string PassengerPhone { get; set; } = null!;
    public string Route { get; set; } = null!;
    public DateTime JourneyDate { get; set; }
    public List<TransactionDto> Transactions { get; set; } = new();
}

public class TransactionDto
{
    public Guid Id { get; set; }
    public string GatewayTransactionId { get; set; } = null!;
    public string Gateway { get; set; } = null!;
    public string Action { get; set; } = null!;
    public decimal Amount { get; set; }
    public bool IsSuccess { get; set; }
    public string? ResponseCode { get; set; }
    public string? ResponseMessage { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class ProcessPaymentDto
{
    public Guid TicketId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentGateway Gateway { get; set; }
    public string? CardNumber { get; set; } // For card payments (masked)
    public string? MobileNumber { get; set; } // For mobile banking
    public string? ReturnUrl { get; set; } // For gateway redirects
}

public class PaymentResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public Guid? PaymentId { get; set; }
    public string? TransactionId { get; set; }
    public PaymentStatus Status { get; set; }
    public string? RedirectUrl { get; set; } // For gateway redirects
    public PaymentDto? Payment { get; set; }
}

public class RefundRequestDto
{
    public Guid PaymentId { get; set; }
    public decimal RefundAmount { get; set; }
    public string RefundReason { get; set; } = null!;
}

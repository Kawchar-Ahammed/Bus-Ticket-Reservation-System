using BusTicket.Domain.Enums;

namespace BusTicket.Application.Contracts.Services;

/// <summary>
/// Payment gateway abstraction interface
/// Implementations: MockPaymentGateway, SSLCommerzGateway, StripeGateway, etc.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// Gateway type
    /// </summary>
    PaymentGateway GatewayType { get; }

    /// <summary>
    /// Process a payment
    /// </summary>
    Task<PaymentGatewayResponse> ProcessPaymentAsync(PaymentGatewayRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify a payment status
    /// </summary>
    Task<PaymentGatewayResponse> VerifyPaymentAsync(string transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Process a refund
    /// </summary>
    Task<PaymentGatewayResponse> ProcessRefundAsync(string transactionId, decimal amount, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if gateway is available
    /// </summary>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

public class PaymentGatewayRequest
{
    public string TransactionId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BDT";
    public PaymentMethod PaymentMethod { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

public class PaymentGatewayResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public string? GatewayTransactionId { get; set; }
    public PaymentStatus Status { get; set; }
    public string? RedirectUrl { get; set; }
    public string? ResponseCode { get; set; }
    public string? RawResponse { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

using BusTicket.Domain.Common;
using BusTicket.Domain.Enums;

namespace BusTicket.Domain.Entities;

/// <summary>
/// Entity: Transaction log for payment gateway interactions
/// Child entity of Payment aggregate
/// </summary>
public class Transaction : Entity
{
    public Guid PaymentId { get; private set; }
    public string GatewayTransactionId { get; private set; } = null!;
    public PaymentGateway Gateway { get; private set; }
    public string Action { get; private set; } = null!; // "charge", "refund", "verify"
    public decimal Amount { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? ResponseCode { get; private set; }
    public string? ResponseMessage { get; private set; }
    public string? RawResponse { get; private set; } // Full JSON response
    public DateTime ProcessedAt { get; private set; }

    // Navigation properties
    public virtual Payment Payment { get; private set; } = null!;

    private Transaction() { } // EF Core constructor

    private Transaction(
        Guid paymentId,
        string gatewayTransactionId,
        PaymentGateway gateway,
        string action,
        decimal amount)
    {
        PaymentId = paymentId;
        GatewayTransactionId = gatewayTransactionId;
        Gateway = gateway;
        Action = action;
        Amount = amount;
        ProcessedAt = DateTime.UtcNow;
        IsSuccess = false;
    }

    public static Transaction Create(
        Guid paymentId,
        string gatewayTransactionId,
        PaymentGateway gateway,
        string action,
        decimal amount)
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentException("Payment ID cannot be empty", nameof(paymentId));

        if (string.IsNullOrWhiteSpace(gatewayTransactionId))
            throw new ArgumentException("Gateway transaction ID is required", nameof(gatewayTransactionId));

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action is required", nameof(action));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        return new Transaction(paymentId, gatewayTransactionId, gateway, action, amount);
    }

    public void MarkAsSuccess(string? responseCode = null, string? responseMessage = null, string? rawResponse = null)
    {
        IsSuccess = true;
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        RawResponse = rawResponse;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string? responseCode = null, string? responseMessage = null, string? rawResponse = null)
    {
        IsSuccess = false;
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
        RawResponse = rawResponse;
        UpdatedAt = DateTime.UtcNow;
    }
}

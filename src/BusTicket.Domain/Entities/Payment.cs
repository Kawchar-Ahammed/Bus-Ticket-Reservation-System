using BusTicket.Domain.Common;
using BusTicket.Domain.Enums;
using BusTicket.Domain.ValueObjects;

namespace BusTicket.Domain.Entities;

/// <summary>
/// Entity: Payment for ticket booking
/// Aggregate Root
/// </summary>
public class Payment : Entity, IAggregateRoot
{
    public Guid TicketId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentGateway Gateway { get; private set; }
    public string? TransactionId { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    public decimal RefundAmount { get; private set; }
    public DateTime? RefundDate { get; private set; }
    public string? RefundReason { get; private set; }
    public string? FailureReason { get; private set; }
    public string? GatewayResponse { get; private set; } // JSON response from gateway

    // Navigation properties
    public virtual Ticket Ticket { get; private set; } = null!;
    public virtual ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    private Payment() { } // EF Core constructor

    private Payment(
        Guid ticketId,
        Money amount,
        PaymentMethod paymentMethod,
        PaymentGateway gateway,
        string? transactionId = null)
    {
        TicketId = ticketId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        Gateway = gateway;
        TransactionId = transactionId ?? GenerateTransactionId();
        Status = PaymentStatus.Pending;
        RefundAmount = 0;
    }

    public static Payment Create(
        Guid ticketId,
        Money amount,
        PaymentMethod paymentMethod,
        PaymentGateway gateway,
        string? transactionId = null)
    {
        if (ticketId == Guid.Empty)
            throw new ArgumentException("Ticket ID cannot be empty", nameof(ticketId));

        if (amount == null || amount.Amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero", nameof(amount));

        return new Payment(ticketId, amount, paymentMethod, gateway, transactionId);
    }

    public void MarkAsProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot mark payment as processing. Current status: {Status}");

        Status = PaymentStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted(string? gatewayTransactionId = null, string? gatewayResponse = null)
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Payment is already completed");

        if (Status == PaymentStatus.Refunded || Status == PaymentStatus.PartiallyRefunded)
            throw new InvalidOperationException("Cannot complete a refunded payment");

        Status = PaymentStatus.Completed;
        PaymentDate = DateTime.UtcNow;
        GatewayTransactionId = gatewayTransactionId;
        GatewayResponse = gatewayResponse;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string failureReason, string? gatewayResponse = null)
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Cannot mark completed payment as failed");

        Status = PaymentStatus.Failed;
        FailureReason = failureReason;
        GatewayResponse = gatewayResponse;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCancelled()
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed payment. Use refund instead.");

        Status = PaymentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ProcessRefund(decimal refundAmount, string refundReason)
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Can only refund completed payments");

        if (refundAmount <= 0)
            throw new ArgumentException("Refund amount must be greater than zero", nameof(refundAmount));

        if (refundAmount > Amount.Amount)
            throw new ArgumentException("Refund amount cannot exceed payment amount", nameof(refundAmount));

        var totalRefunded = RefundAmount + refundAmount;
        if (totalRefunded > Amount.Amount)
            throw new InvalidOperationException("Total refund amount exceeds payment amount");

        RefundAmount = totalRefunded;
        RefundDate = DateTime.UtcNow;
        RefundReason = refundReason;

        Status = RefundAmount >= Amount.Amount
            ? PaymentStatus.Refunded
            : PaymentStatus.PartiallyRefunded;

        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTransaction(Transaction transaction)
    {
        if (transaction.PaymentId != Id)
            throw new ArgumentException("Transaction does not belong to this payment");

        Transactions.Add(transaction);
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateTransactionId()
    {
        return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    public bool CanBeRefunded()
    {
        return Status == PaymentStatus.Completed && RefundAmount < Amount.Amount;
    }

    public decimal GetRemainingRefundableAmount()
    {
        return Status == PaymentStatus.Completed ? Amount.Amount - RefundAmount : 0;
    }
}

using BusTicket.Application.Contracts.Services;
using BusTicket.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace BusTicket.Infrastructure.Services;

/// <summary>
/// Mock payment gateway for testing
/// Simulates payment processing without actual gateway integration
/// </summary>
public class MockPaymentGateway : IPaymentGateway
{
    private readonly ILogger<MockPaymentGateway> _logger;
    private static readonly Random _random = new();

    public PaymentGateway GatewayType => PaymentGateway.Mock;

    public MockPaymentGateway(ILogger<MockPaymentGateway> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentGatewayResponse> ProcessPaymentAsync(
        PaymentGatewayRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockGateway: Processing payment {TransactionId} for amount {Amount} {Currency}",
            request.TransactionId, request.Amount, request.Currency);

        // Simulate processing delay
        await Task.Delay(1000, cancellationToken);

        // 95% success rate in mock
        var isSuccess = _random.Next(100) < 95;

        var response = new PaymentGatewayResponse
        {
            IsSuccess = isSuccess,
            Message = isSuccess ? "Payment processed successfully" : "Payment failed - Insufficient funds",
            GatewayTransactionId = $"MOCK-{Guid.NewGuid().ToString().Substring(0, 12).ToUpper()}",
            Status = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed,
            ResponseCode = isSuccess ? "00" : "51",
            RawResponse = $"{{\"mock\":true,\"txnId\":\"{request.TransactionId}\",\"status\":\"{(isSuccess ? "success" : "failed")}\"}}"
        };

        _logger.LogInformation("MockGateway: Payment {TransactionId} result: {Status}",
            request.TransactionId, response.Status);

        return response;
    }

    public async Task<PaymentGatewayResponse> VerifyPaymentAsync(
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockGateway: Verifying payment {TransactionId}", transactionId);

        await Task.Delay(500, cancellationToken);

        return new PaymentGatewayResponse
        {
            IsSuccess = true,
            Message = "Payment verified successfully",
            GatewayTransactionId = transactionId,
            Status = PaymentStatus.Completed,
            ResponseCode = "00"
        };
    }

    public async Task<PaymentGatewayResponse> ProcessRefundAsync(
        string transactionId,
        decimal amount,
        string reason,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MockGateway: Processing refund for {TransactionId}, amount: {Amount}, reason: {Reason}",
            transactionId, amount, reason);

        await Task.Delay(1000, cancellationToken);

        return new PaymentGatewayResponse
        {
            IsSuccess = true,
            Message = $"Refund of {amount} processed successfully",
            GatewayTransactionId = $"REFUND-{Guid.NewGuid().ToString().Substring(0, 12).ToUpper()}",
            Status = PaymentStatus.Refunded,
            ResponseCode = "00",
            RawResponse = $"{{\"mock\":true,\"refundTxnId\":\"REFUND-{transactionId}\",\"amount\":{amount}}}"
        };
    }

    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}

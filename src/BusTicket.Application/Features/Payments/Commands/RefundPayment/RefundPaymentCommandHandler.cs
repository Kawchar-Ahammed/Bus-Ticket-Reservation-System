using BusTicket.Application.Contracts.Features.Payments;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Contracts.Services;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using BusTicket.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicket.Application.Features.Payments.Commands.RefundPayment;

public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, PaymentResponseDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RefundPaymentCommandHandler> _logger;

    public RefundPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        IUnitOfWork unitOfWork,
        ILogger<RefundPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PaymentResponseDto> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing refund for payment {PaymentId}, amount: {Amount}",
            request.PaymentId, request.RefundAmount);

        // 1. Get payment with details
        var payment = await _paymentRepository.GetPaymentWithDetailsAsync(request.PaymentId, cancellationToken);
        if (payment == null)
            throw new NotFoundException($"Payment with ID {request.PaymentId} not found");

        // 2. Validate refund is possible
        if (!payment.CanBeRefunded())
            throw new ValidationException($"Payment cannot be refunded. Current status: {payment.Status}");

        var refundableAmount = payment.GetRemainingRefundableAmount();
        if (request.RefundAmount > refundableAmount)
            throw new ValidationException($"Refund amount ({request.RefundAmount}) exceeds refundable amount ({refundableAmount})");

        try
        {
            // 3. Process refund through gateway
            var gatewayResponse = await _paymentGateway.ProcessRefundAsync(
                payment.GatewayTransactionId ?? payment.TransactionId!,
                request.RefundAmount,
                request.RefundReason,
                cancellationToken
            );

            // 4. Create refund transaction record
            var transaction = Transaction.Create(
                paymentId: payment.Id,
                gatewayTransactionId: gatewayResponse.GatewayTransactionId ?? "N/A",
                gateway: payment.Gateway,
                action: "refund",
                amount: request.RefundAmount
            );

            if (gatewayResponse.IsSuccess)
            {
                transaction.MarkAsSuccess(
                    gatewayResponse.ResponseCode,
                    gatewayResponse.Message,
                    gatewayResponse.RawResponse
                );

                // 5. Update payment with refund
                payment.ProcessRefund(request.RefundAmount, request.RefundReason);
            }
            else
            {
                transaction.MarkAsFailed(
                    gatewayResponse.ResponseCode,
                    gatewayResponse.Message,
                    gatewayResponse.RawResponse
                );
            }

            payment.AddTransaction(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Refund processed for payment {PaymentId}, status: {Status}",
                payment.Id, payment.Status);

            return new PaymentResponseDto
            {
                Success = gatewayResponse.IsSuccess,
                Message = gatewayResponse.Message,
                PaymentId = payment.Id,
                TransactionId = payment.TransactionId,
                Status = payment.Status
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund for payment {PaymentId}", request.PaymentId);
            throw;
        }
    }
}

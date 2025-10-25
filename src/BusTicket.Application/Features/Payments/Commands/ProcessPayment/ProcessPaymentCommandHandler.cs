using BusTicket.Application.Contracts.Features.Payments;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Contracts.Services;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using BusTicket.Domain.Exceptions;
using BusTicket.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicket.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentResponseDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        ITicketRepository ticketRepository,
        IPaymentGateway paymentGateway,
        IUnitOfWork unitOfWork,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _ticketRepository = ticketRepository;
        _paymentGateway = paymentGateway;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PaymentResponseDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment for ticket {TicketId}", request.TicketId);

        // 1. Verify ticket exists
        var ticket = await _ticketRepository.GetTicketWithDetailsAsync(request.TicketId, cancellationToken);
        if (ticket == null)
            throw new NotFoundException($"Ticket with ID {request.TicketId} not found");

        // 2. Check if ticket is already confirmed
        if (ticket.IsConfirmed)
            throw new ValidationException("Ticket is already confirmed and paid");

        // 3. Check if ticket is cancelled
        if (ticket.IsCancelled)
            throw new ValidationException("Cannot pay for a cancelled ticket");

        // 4. Check if payment already exists for this ticket
        var existingPayment = await _paymentRepository.GetByTicketIdAsync(request.TicketId, cancellationToken);
        if (existingPayment != null && existingPayment.Status == PaymentStatus.Completed)
        {
            return new PaymentResponseDto
            {
                Success = false,
                Message = "Payment already completed for this ticket",
                PaymentId = existingPayment.Id,
                TransactionId = existingPayment.TransactionId,
                Status = existingPayment.Status
            };
        }

        // 5. Verify amount matches ticket fare
        if (request.Amount != ticket.Fare.Amount)
            throw new ValidationException($"Payment amount ({request.Amount}) does not match ticket fare ({ticket.Fare.Amount})");

        // 6. Create payment entity
        var money = Money.Create(request.Amount, ticket.Fare.Currency);
        var payment = Payment.Create(
            ticketId: request.TicketId,
            amount: money,
            paymentMethod: request.PaymentMethod,
            gateway: request.Gateway
        );

        payment.MarkAsProcessing();
        await _paymentRepository.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            // 7. Process payment through gateway
            var gatewayRequest = new PaymentGatewayRequest
            {
                TransactionId = payment.TransactionId!,
                Amount = request.Amount,
                Currency = ticket.Fare.Currency,
                PaymentMethod = request.PaymentMethod,
                CustomerName = ticket.Passenger.Name,
                CustomerEmail = ticket.Passenger.Email,
                CustomerPhone = request.CustomerPhone ?? ticket.Passenger.PhoneNumber.Value,
                ReturnUrl = request.ReturnUrl,
                Metadata = new Dictionary<string, string>
                {
                    { "TicketId", ticket.Id.ToString() },
                    { "TicketNumber", ticket.TicketNumber },
                    { "PassengerId", ticket.Passenger.Id.ToString() }
                }
            };

            var gatewayResponse = await _paymentGateway.ProcessPaymentAsync(gatewayRequest, cancellationToken);

            // 8. Create transaction record
            var transaction = Transaction.Create(
                paymentId: payment.Id,
                gatewayTransactionId: gatewayResponse.GatewayTransactionId ?? "N/A",
                gateway: request.Gateway,
                action: "charge",
                amount: request.Amount
            );

            if (gatewayResponse.IsSuccess)
            {
                transaction.MarkAsSuccess(
                    gatewayResponse.ResponseCode,
                    gatewayResponse.Message,
                    gatewayResponse.RawResponse
                );
                payment.MarkAsCompleted(gatewayResponse.GatewayTransactionId, gatewayResponse.RawResponse);
                
                // Confirm the ticket
                ticket.Confirm();
            }
            else
            {
                transaction.MarkAsFailed(
                    gatewayResponse.ResponseCode,
                    gatewayResponse.Message,
                    gatewayResponse.RawResponse
                );
                payment.MarkAsFailed(gatewayResponse.Message, gatewayResponse.RawResponse);
            }

            payment.AddTransaction(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment {PaymentId} processed with status {Status}", payment.Id, payment.Status);

            return new PaymentResponseDto
            {
                Success = gatewayResponse.IsSuccess,
                Message = gatewayResponse.Message,
                PaymentId = payment.Id,
                TransactionId = payment.TransactionId,
                Status = payment.Status,
                RedirectUrl = gatewayResponse.RedirectUrl,
                Payment = MapToDto(payment)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for ticket {TicketId}", request.TicketId);
            payment.MarkAsFailed($"Error: {ex.Message}");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    private PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            TicketId = payment.TicketId,
            TicketNumber = payment.Ticket?.TicketNumber ?? "",
            Amount = payment.Amount.Amount,
            Currency = payment.Amount.Currency,
            PaymentMethod = payment.PaymentMethod,
            PaymentMethodName = payment.PaymentMethod.ToString(),
            Status = payment.Status,
            StatusName = payment.Status.ToString(),
            Gateway = payment.Gateway,
            GatewayName = payment.Gateway.ToString(),
            TransactionId = payment.TransactionId,
            GatewayTransactionId = payment.GatewayTransactionId,
            PaymentDate = payment.PaymentDate,
            RefundAmount = payment.RefundAmount,
            RefundDate = payment.RefundDate,
            RefundReason = payment.RefundReason,
            FailureReason = payment.FailureReason,
            CreatedAt = payment.CreatedAt
        };
    }
}

using BusTicket.Application.Contracts.Features.Payments;
using BusTicket.Application.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicket.Application.Features.Payments.Queries.GetPaymentByTicketId;

public class GetPaymentByTicketIdQueryHandler : IRequestHandler<GetPaymentByTicketIdQuery, PaymentDetailsDto?>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<GetPaymentByTicketIdQueryHandler> _logger;

    public GetPaymentByTicketIdQueryHandler(
        IPaymentRepository paymentRepository,
        ILogger<GetPaymentByTicketIdQueryHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<PaymentDetailsDto?> Handle(GetPaymentByTicketIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting payment for ticket {TicketId}", request.TicketId);

        var payment = await _paymentRepository.GetByTicketIdAsync(request.TicketId, cancellationToken);
        if (payment == null)
            return null;

        // Load payment with full details
        payment = await _paymentRepository.GetPaymentWithDetailsAsync(payment.Id, cancellationToken);
        if (payment == null)
            return null;

        return new PaymentDetailsDto
        {
            Id = payment.Id,
            TicketId = payment.TicketId,
            TicketNumber = payment.Ticket.TicketNumber,
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
            CreatedAt = payment.CreatedAt,
            PassengerName = payment.Ticket.Passenger.Name,
            PassengerPhone = payment.Ticket.Passenger.PhoneNumber.Value,
            Route = $"{payment.Ticket.BusSchedule.Route.FromLocation.City} → {payment.Ticket.BusSchedule.Route.ToLocation.City}",
            JourneyDate = payment.Ticket.BusSchedule.JourneyDate,
            Transactions = payment.Transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                GatewayTransactionId = t.GatewayTransactionId,
                Gateway = t.Gateway.ToString(),
                Action = t.Action,
                Amount = t.Amount,
                IsSuccess = t.IsSuccess,
                ResponseCode = t.ResponseCode,
                ResponseMessage = t.ResponseMessage,
                ProcessedAt = t.ProcessedAt
            }).ToList()
        };
    }
}

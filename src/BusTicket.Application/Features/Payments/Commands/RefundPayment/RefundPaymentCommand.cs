using BusTicket.Application.Contracts.Features.Payments;
using MediatR;

namespace BusTicket.Application.Features.Payments.Commands.RefundPayment;

public record RefundPaymentCommand(
    Guid PaymentId,
    decimal RefundAmount,
    string RefundReason
) : IRequest<PaymentResponseDto>;

using BusTicket.Application.Contracts.Features.Payments;
using BusTicket.Domain.Enums;
using MediatR;

namespace BusTicket.Application.Features.Payments.Commands.ProcessPayment;

public record ProcessPaymentCommand(
    Guid TicketId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    PaymentGateway Gateway,
    string? CustomerPhone = null,
    string? ReturnUrl = null
) : IRequest<PaymentResponseDto>;

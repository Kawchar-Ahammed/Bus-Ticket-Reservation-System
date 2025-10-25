using BusTicket.Application.Contracts.Features.Payments;
using MediatR;

namespace BusTicket.Application.Features.Payments.Queries.GetPaymentByTicketId;

public record GetPaymentByTicketIdQuery(Guid TicketId) : IRequest<PaymentDetailsDto?>;

using BusTicket.Application.Contracts.Features.Payments;
using BusTicket.Application.Features.Payments.Commands.ProcessPayment;
using BusTicket.Application.Features.Payments.Commands.RefundPayment;
using BusTicket.Application.Features.Payments.Queries.GetPaymentByTicketId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicket.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Process payment for a ticket
    /// </summary>
    [HttpPost("process")]
    [AllowAnonymous] // Allow passengers to pay without authentication
    [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentResponseDto>> ProcessPayment([FromBody] ProcessPaymentDto dto)
    {
        var command = new ProcessPaymentCommand(
            dto.TicketId,
            dto.Amount,
            dto.PaymentMethod,
            dto.Gateway,
            dto.MobileNumber,
            dto.ReturnUrl
        );

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Process refund for a payment
    /// </summary>
    [HttpPost("refund")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(PaymentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentResponseDto>> ProcessRefund([FromBody] RefundRequestDto dto)
    {
        var command = new RefundPaymentCommand(
            dto.PaymentId,
            dto.RefundAmount,
            dto.RefundReason
        );

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get payment details by ticket ID
    /// </summary>
    [HttpGet("ticket/{ticketId}")]
    [AllowAnonymous] // Allow passengers to view their payment details
    [ProducesResponseType(typeof(PaymentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDetailsDto>> GetPaymentByTicketId(Guid ticketId)
    {
        var query = new GetPaymentByTicketIdQuery(ticketId);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound($"No payment found for ticket {ticketId}");

        return Ok(result);
    }
}

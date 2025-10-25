using BusTicket.Application.Commands;
using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusTicket.WebApi.Controllers;

/// <summary>
/// API controller for managing booking history and tracking
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BookingHistoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingHistoryController> _logger;

    public BookingHistoryController(
        IMediator mediator,
        ILogger<BookingHistoryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all bookings for a phone number
    /// </summary>
    /// <param name="phoneNumber">Passenger phone number</param>
    /// <returns>List of bookings</returns>
    [HttpGet("by-phone/{phoneNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBookingsByPhoneNumber(string phoneNumber)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return BadRequest(new { message = "Phone number is required" });
            }

            _logger.LogInformation("Getting bookings for phone number: {PhoneNumber}", phoneNumber);

            var query = new GetBookingsByPhoneNumberQuery(phoneNumber);
            var bookings = await _mediator.Send(query);

            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookings for phone number: {PhoneNumber}", phoneNumber);
            return StatusCode(500, new { message = "An error occurred while retrieving bookings" });
        }
    }

    /// <summary>
    /// Get booking details by ticket number
    /// </summary>
    /// <param name="ticketNumber">Ticket number</param>
    /// <returns>Booking details</returns>
    [HttpGet("by-ticket/{ticketNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBookingByTicketNumber(string ticketNumber)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ticketNumber))
            {
                return BadRequest(new { message = "Ticket number is required" });
            }

            _logger.LogInformation("Getting booking details for ticket: {TicketNumber}", ticketNumber);

            var query = new GetBookingByTicketNumberQuery(ticketNumber);
            var booking = await _mediator.Send(query);

            if (booking == null)
            {
                return NotFound(new { message = $"Booking not found for ticket number: {ticketNumber}" });
            }

            return Ok(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booking for ticket: {TicketNumber}", ticketNumber);
            return StatusCode(500, new { message = "An error occurred while retrieving booking details" });
        }
    }

    /// <summary>
    /// Cancel a booking
    /// </summary>
    /// <param name="request">Cancellation request</param>
    /// <returns>Cancellation result</returns>
    [HttpPost("cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelBooking([FromBody] CancelBookingRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.TicketNumber))
            {
                return BadRequest(new { message = "Ticket number is required" });
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return BadRequest(new { message = "Phone number is required" });
            }

            _logger.LogInformation(
                "Cancelling booking: {TicketNumber} for phone: {PhoneNumber}",
                request.TicketNumber,
                request.PhoneNumber);

            var command = new CancelBookingCommand(
                request.TicketNumber,
                request.PhoneNumber,
                request.CancellationReason);

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking: {TicketNumber}", request.TicketNumber);
            return StatusCode(500, new { message = "An error occurred while cancelling the booking" });
        }
    }
}

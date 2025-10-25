using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicket.WebApi.Controllers;

/// <summary>
/// Controller for booking operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(
        IBookingService bookingService,
        ILogger<BookingController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Get seat plan for a specific bus schedule
    /// </summary>
    /// <param name="scheduleId">Bus schedule ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Seat plan with availability</returns>
    /// <response code="200">Returns the seat plan</response>
    /// <response code="404">If the schedule is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("seat-plan/{scheduleId:guid}")]
    [ProducesResponseType(typeof(SeatPlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SeatPlanDto>> GetSeatPlan(
        Guid scheduleId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting seat plan for schedule {ScheduleId}", scheduleId);

            var result = await _bookingService.GetSeatPlanAsync(scheduleId, cancellationToken);

            if (result == null)
            {
                _logger.LogWarning("Schedule {ScheduleId} not found", scheduleId);
                return NotFound(new { message = $"Schedule with ID {scheduleId} not found." });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting seat plan for schedule {ScheduleId}", scheduleId);
            return StatusCode(500, new { message = "An error occurred while getting seat plan." });
        }
    }

    /// <summary>
    /// Book a seat on a bus
    /// </summary>
    /// <param name="input">Booking input data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Booking result with ticket information</returns>
    /// <response code="200">Returns the booking result</response>
    /// <response code="400">If the booking input is invalid or booking failed</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("book-seat")]
    [ProducesResponseType(typeof(BookSeatResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookSeatResultDto>> BookSeat(
        [FromBody] BookSeatInputDto input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Booking seat {SeatId} for passenger {PassengerName}",
                input.SeatId, input.PassengerName);

            var result = await _bookingService.BookSeatAsync(input, cancellationToken);

            if (!result.Success)
            {
                _logger.LogWarning(
                    "Booking failed for seat {SeatId}: {Message}",
                    input.SeatId, result.Message);

                return BadRequest(result);
            }

            _logger.LogInformation(
                "Seat {SeatId} booked successfully. Ticket number: {TicketNumber}",
                input.SeatId, result.TicketNumber);

            return Ok(result);
        }
        catch (FluentValidation.ValidationException validationEx)
        {
            _logger.LogWarning(validationEx, "Validation failed for booking request");
            
            var errors = validationEx.Errors
                .Select(e => new { Property = e.PropertyName, Error = e.ErrorMessage })
                .ToList();

            return BadRequest(new { message = "Validation failed", errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking seat {SeatId}", input.SeatId);
            return StatusCode(500, new { message = "An error occurred while booking the seat." });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>API status</returns>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<object> HealthCheck()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "Bus Ticket Booking API"
        });
    }
}

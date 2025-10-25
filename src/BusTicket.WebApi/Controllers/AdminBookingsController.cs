using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicket.WebApi.Controllers;

[ApiController]
[Route("api/admin/bookings")]
[Authorize] // Requires admin authentication
public class AdminBookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminBookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all bookings with optional filters (Admin only)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<AdminBookingDto>>> GetAll(
        [FromQuery] Guid? companyId,
        [FromQuery] string? bookingStatus,
        [FromQuery] DateTime? journeyDateFrom,
        [FromQuery] DateTime? journeyDateTo,
        [FromQuery] DateTime? bookingDateFrom,
        [FromQuery] DateTime? bookingDateTo,
        [FromQuery] string? searchTerm)
    {
        var query = new GetAllBookingsForAdminQuery
        {
            CompanyId = companyId,
            BookingStatus = bookingStatus,
            JourneyDateFrom = journeyDateFrom,
            JourneyDateTo = journeyDateTo,
            BookingDateFrom = bookingDateFrom,
            BookingDateTo = bookingDateTo,
            SearchTerm = searchTerm
        };

        var bookings = await _mediator.Send(query);
        return Ok(bookings);
    }

    /// <summary>
    /// Get booking by ID (Admin only)
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminBookingDetailDto>> GetById(Guid id)
    {
        var query = new GetBookingByIdForAdminQuery(id);
        var booking = await _mediator.Send(query);

        if (booking == null)
            return NotFound(new { message = $"Booking with ID {id} not found" });

        return Ok(booking);
    }

    /// <summary>
    /// Get booking by ticket number (Admin only)
    /// </summary>
    [HttpGet("ticket/{ticketNumber}")]
    public async Task<ActionResult<AdminBookingDetailDto>> GetByTicketNumber(string ticketNumber)
    {
        var query = new GetBookingByTicketNumberForAdminQuery(ticketNumber);
        var booking = await _mediator.Send(query);

        if (booking == null)
            return NotFound(new { message = $"Booking with ticket number {ticketNumber} not found" });

        return Ok(booking);
    }
}

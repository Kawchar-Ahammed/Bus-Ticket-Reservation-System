using BusTicket.Application.Features.BusSchedules.Commands;
using BusTicket.Application.Features.BusSchedules.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicket.WebApi.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize]
public class BusSchedulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusSchedulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all bus schedules
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schedules = await _mediator.Send(new GetAllBusSchedulesQuery());
        return Ok(schedules);
    }

    /// <summary>
    /// Get bus schedule by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var schedule = await _mediator.Send(new GetBusScheduleByIdQuery(id));
        
        if (schedule == null)
            return NotFound(new { message = $"Bus schedule with ID {id} not found" });

        return Ok(schedule);
    }

    /// <summary>
    /// Create a new bus schedule
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBusScheduleCommand command)
    {
        var scheduleId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = scheduleId }, new { id = scheduleId });
    }

    /// <summary>
    /// Update an existing bus schedule
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBusScheduleCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { message = "Schedule ID mismatch" });

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete a bus schedule
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteBusScheduleCommand(id));
        return NoContent();
    }
}

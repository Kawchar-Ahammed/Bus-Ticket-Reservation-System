using BusTicket.Application.Features.Buses.Commands;
using BusTicket.Application.Features.Buses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicket.WebApi.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize]
public class BusesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all buses
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var buses = await _mediator.Send(new GetAllBusesQuery());
        return Ok(buses);
    }

    /// <summary>
    /// Get bus by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var bus = await _mediator.Send(new GetBusByIdQuery(id));
        
        if (bus == null)
            return NotFound(new { message = $"Bus with ID {id} not found" });

        return Ok(bus);
    }

    /// <summary>
    /// Create a new bus
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBusCommand command)
    {
        var busId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = busId }, new { id = busId });
    }

    /// <summary>
    /// Update an existing bus
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBusCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { message = "Bus ID mismatch" });

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete a bus
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteBusCommand(id));
        return NoContent();
    }
}

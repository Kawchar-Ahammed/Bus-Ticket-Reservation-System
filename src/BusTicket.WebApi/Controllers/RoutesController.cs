using BusTicket.Application.Features.Routes.Commands;
using BusTicket.Application.Features.Routes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicket.WebApi.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize]
public class RoutesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoutesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all routes
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var routes = await _mediator.Send(new GetAllRoutesQuery());
        return Ok(routes);
    }

    /// <summary>
    /// Get route by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var route = await _mediator.Send(new GetRouteByIdQuery(id));
        
        if (route == null)
            return NotFound(new { message = $"Route with ID {id} not found" });

        return Ok(route);
    }

    /// <summary>
    /// Create a new route
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRouteCommand command)
    {
        var routeId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = routeId }, new { id = routeId });
    }

    /// <summary>
    /// Update an existing route
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRouteCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { message = "Route ID mismatch" });

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete a route
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteRouteCommand(id));
        return NoContent();
    }
}

using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Features.AdminUsers.Commands.ActivateAdminUser;
using BusTicket.Application.Features.AdminUsers.Commands.ChangeAdminUserRole;
using BusTicket.Application.Features.AdminUsers.Commands.CreateAdminUser;
using BusTicket.Application.Features.AdminUsers.Commands.DeactivateAdminUser;
using BusTicket.Application.Features.AdminUsers.Commands.UpdateAdminUser;
using BusTicket.Application.Features.AdminUsers.Queries.GetAdminUserById;
using BusTicket.Application.Features.AdminUsers.Queries.GetAllAdminUsers;
using BusTicket.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicket.WebApi.Controllers;

/// <summary>
/// Admin Users Management Controller - SuperAdmin only
/// </summary>
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "SuperAdmin")]
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all admin users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<AdminUserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AdminUserDto>>> GetAll(
        [FromQuery] AdminRole? role = null,
        [FromQuery] bool? isActive = null)
    {
        var query = new GetAllAdminUsersQuery(role, isActive);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get admin user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AdminUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminUserDto>> GetById(Guid id)
    {
        var query = new GetAdminUserByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create new admin user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateAdminUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = userId }, userId);
    }

    /// <summary>
    /// Update admin user profile
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateAdminUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Change admin user role
    /// </summary>
    [HttpPut("{id}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangeRole(Guid id, [FromBody] ChangeAdminUserRoleCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Deactivate admin user
    /// </summary>
    [HttpPut("{id}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Deactivate(Guid id)
    {
        var command = new DeactivateAdminUserCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Activate admin user
    /// </summary>
    [HttpPut("{id}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Activate(Guid id)
    {
        var command = new ActivateAdminUserCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}

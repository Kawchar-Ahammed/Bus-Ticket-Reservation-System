using BusTicket.Application.Features.Companies.Commands.CreateCompany;
using BusTicket.Application.Features.Companies.Commands.DeleteCompany;
using BusTicket.Application.Features.Companies.Commands.UpdateCompany;
using BusTicket.Application.Features.Companies.Queries.GetAllCompanies;
using BusTicket.Application.Features.Companies.Queries.GetCompanyById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicket.WebApi.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all companies
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CompanyDto>>> GetAll()
    {
        var companies = await _mediator.Send(new GetAllCompaniesQuery());
        return Ok(companies);
    }

    /// <summary>
    /// Get company by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDetailDto>> GetById(Guid id)
    {
        var company = await _mediator.Send(new GetCompanyByIdQuery(id));
        
        if (company == null)
            return NotFound(new { message = $"Company with ID {id} not found" });
        
        return Ok(company);
    }

    /// <summary>
    /// Create a new company
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCompanyCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Update an existing company
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateCompanyCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { message = "ID mismatch" });
        
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete a company
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCompanyCommand(id));
        return NoContent();
    }
}

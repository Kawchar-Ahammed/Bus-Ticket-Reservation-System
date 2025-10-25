using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicket.WebApi.Controllers;

/// <summary>
/// Controller for bus search operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BusSearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<BusSearchController> _logger;

    public BusSearchController(
        ISearchService searchService,
        ILogger<BusSearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Search available buses by route and date
    /// </summary>
    /// <param name="from">Departure city</param>
    /// <param name="to">Destination city</param>
    /// <param name="journeyDate">Journey date (YYYY-MM-DD)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available buses</returns>
    /// <response code="200">Returns the list of available buses</response>
    /// <response code="400">If the request parameters are invalid</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IReadOnlyList<AvailableBusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<AvailableBusDto>>> SearchAvailableBuses(
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] DateTime journeyDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Searching buses from {From} to {To} on {Date}",
                from, to, journeyDate.Date);

            var result = await _searchService.SearchAvailableBusesAsync(
                from, to, journeyDate, cancellationToken);

            _logger.LogInformation(
                "Found {Count} available buses from {From} to {To}",
                result.Count, from, to);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching buses from {From} to {To}", from, to);
            return StatusCode(500, new { message = "An error occurred while searching buses." });
        }
    }

    /// <summary>
    /// Get popular routes
    /// </summary>
    /// <returns>List of popular routes</returns>
    /// <response code="200">Returns the list of popular routes</response>
    [HttpGet("popular-routes")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetPopularRoutes()
    {
        // Return hardcoded popular routes for now
        var routes = new[]
        {
            new { From = "Dhaka", To = "Chittagong", Distance = 264 },
            new { From = "Dhaka", To = "Cox's Bazar", Distance = 397 },
            new { From = "Dhaka", To = "Sylhet", Distance = 244 },
            new { From = "Dhaka", To = "Rajshahi", Distance = 256 },
            new { From = "Chittagong", To = "Cox's Bazar", Distance = 150 }
        };

        return Ok(routes);
    }
}

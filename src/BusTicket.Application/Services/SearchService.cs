using BusTicket.Application.Commands;
using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Services;
using BusTicket.Application.Queries;
using MediatR;

namespace BusTicket.Application.Services;

/// <summary>
/// Application Service for searching buses
/// Acts as a facade over MediatR commands/queries
/// </summary>
public class SearchService : ISearchService
{
    private readonly IMediator _mediator;

    public SearchService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<List<AvailableBusDto>> SearchAvailableBusesAsync(
        string from,
        string to,
        DateTime journeyDate,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchAvailableBusesQuery(from, to, journeyDate);
        return await _mediator.Send(query, cancellationToken);
    }
}

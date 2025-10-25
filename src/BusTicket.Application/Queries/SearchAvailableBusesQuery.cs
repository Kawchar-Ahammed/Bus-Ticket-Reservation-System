using BusTicket.Application.Contracts.DTOs;
using MediatR;

namespace BusTicket.Application.Queries;

/// <summary>
/// Query to search for available buses
/// </summary>
public record SearchAvailableBusesQuery(
    string From,
    string To,
    DateTime JourneyDate
) : IRequest<List<AvailableBusDto>>;

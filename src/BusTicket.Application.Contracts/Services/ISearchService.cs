using BusTicket.Application.Contracts.DTOs;

namespace BusTicket.Application.Contracts.Services;

/// <summary>
/// Application Service Interface for searching buses
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Searches for available buses between two cities on a specific date
    /// </summary>
    /// <param name="from">Departure city</param>
    /// <param name="to">Destination city</param>
    /// <param name="journeyDate">Journey date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available buses</returns>
    Task<List<AvailableBusDto>> SearchAvailableBusesAsync(
        string from,
        string to,
        DateTime journeyDate,
        CancellationToken cancellationToken = default);
}

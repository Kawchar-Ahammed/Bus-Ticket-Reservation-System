using BusTicket.Domain.Entities;

namespace BusTicket.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for BusSchedule aggregate
/// </summary>
public interface IBusScheduleRepository : IRepository<BusSchedule>
{
    /// <summary>
    /// Finds bus schedules for a specific route and date
    /// </summary>
    Task<List<BusSchedule>> FindByRouteAndDateAsync(
        string fromCity,
        string toCity,
        DateTime journeyDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bus schedule with all related entities (Bus, Route, Seats, Company)
    /// </summary>
    Task<BusSchedule?> GetWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bus schedules with seats for a specific date
    /// </summary>
    Task<List<BusSchedule>> GetSchedulesWithSeatsAsync(
        DateTime journeyDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all schedules with Bus, Route, and Company details
    /// </summary>
    Task<List<BusSchedule>> GetAllWithDetailsAsync();

    /// <summary>
    /// Gets a specific schedule with Bus, Route, and Company details
    /// </summary>
    Task<BusSchedule?> GetByIdWithDetailsAsync(Guid id);

    /// <summary>
    /// Gets a schedule by ID without related entities
    /// </summary>
    Task<BusSchedule?> GetByIdAsync(Guid id);
}

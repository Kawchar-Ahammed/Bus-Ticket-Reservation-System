using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusTicket.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for BusSchedule aggregate
/// </summary>
public class BusScheduleRepository : Repository<BusSchedule>, IBusScheduleRepository
{
    public BusScheduleRepository(BusTicketDbContext context) : base(context)
    {
    }

    public async Task<List<BusSchedule>> FindByRouteAndDateAsync(
        string fromCity,
        string toCity,
        DateTime journeyDate,
        CancellationToken cancellationToken = default)
    {
        // Convert to UTC if not already, and get the date boundaries
        var startOfDay = DateTime.SpecifyKind(journeyDate.Date, DateTimeKind.Utc);
        var endOfDay = startOfDay.AddDays(1);

        return await _dbSet
            .Include(s => s.Bus)
                .ThenInclude(b => b.Company)
            .Include(s => s.Route)
            .Include(s => s.Seats)
            .Where(s => s.Route.FromLocation.City == fromCity &&
                       s.Route.ToLocation.City == toCity &&
                       s.JourneyDate >= startOfDay &&
                       s.JourneyDate < endOfDay &&
                       s.IsActive)
            .OrderBy(s => s.DepartureTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<BusSchedule?> GetWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Bus)
                .ThenInclude(b => b.Company)
            .Include(s => s.Route)
                .ThenInclude(r => r.FromLocation)
            .Include(s => s.Route)
                .ThenInclude(r => r.ToLocation)
            .Include(s => s.Seats)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<BusSchedule>> GetSchedulesWithSeatsAsync(
        DateTime journeyDate,
        CancellationToken cancellationToken = default)
    {
        var startOfDay = journeyDate.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _dbSet
            .Include(s => s.Bus)
                .ThenInclude(b => b.Company)
            .Include(s => s.Route)
            .Where(s => s.JourneyDate >= startOfDay &&
                       s.JourneyDate < endOfDay &&
                       s.IsActive)
            .OrderBy(s => s.DepartureTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BusSchedule>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(s => s.Bus)
                .ThenInclude(b => b.Company)
            .Include(s => s.Route)
            .Include(s => s.Seats)
            .OrderByDescending(s => s.JourneyDate)
            .ThenBy(s => s.DepartureTime)
            .ToListAsync();
    }

    public async Task<BusSchedule?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.Bus)
                .ThenInclude(b => b.Company)
            .Include(s => s.Route)
            .Include(s => s.Seats)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<BusSchedule?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Id == id);
    }
}

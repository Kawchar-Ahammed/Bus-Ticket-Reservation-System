using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using BusTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusTicket.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Seat entity
/// </summary>
public class SeatRepository : ISeatRepository
{
    private readonly BusTicketDbContext _context;
    private readonly DbSet<Seat> _dbSet;

    public SeatRepository(BusTicketDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Seat>();
    }

    public async Task<Seat?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public Task UpdateAsync(Seat entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<List<Seat>> GetSeatsByScheduleIdAsync(
        Guid busScheduleId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.BusScheduleId == busScheduleId)
            .OrderBy(s => s.Row)
            .ThenBy(s => s.Column)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Seat>> GetAvailableSeatsAsync(
        Guid busScheduleId,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _context.BusSchedules
            .FirstOrDefaultAsync(s => s.Id == busScheduleId, cancellationToken);

        if (schedule == null)
            return new List<Seat>();

        return await _dbSet
            .Where(s => EF.Property<Guid>(s, "BusId") == schedule.BusId &&
                       s.Status == SeatStatus.Available)
            .OrderBy(s => s.SeatNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<Seat?> GetSeatWithScheduleAsync(
        Guid seatId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Id == seatId, cancellationToken);
    }

    public async Task<bool> IsSeatAvailableAsync(
        Guid seatId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(
                s => s.Id == seatId && s.Status == SeatStatus.Available,
                cancellationToken);
    }

    public async Task<List<Seat>> GetSeatsByStatusAsync(
        Guid busScheduleId,
        SeatStatus status,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _context.BusSchedules
            .FirstOrDefaultAsync(s => s.Id == busScheduleId, cancellationToken);

        if (schedule == null)
            return new List<Seat>();

        return await _dbSet
            .Where(s => EF.Property<Guid>(s, "BusId") == schedule.BusId &&
                       s.Status == status)
            .OrderBy(s => s.SeatNumber)
            .ToListAsync(cancellationToken);
    }
}

using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusTicket.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Passenger entity
/// </summary>
public class PassengerRepository : IPassengerRepository
{
    private readonly BusTicketDbContext _context;
    private readonly DbSet<Passenger> _dbSet;

    public PassengerRepository(BusTicketDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Passenger>();
    }

    public async Task<Passenger?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<Passenger> AddAsync(Passenger entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(Passenger entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<Passenger?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<Passenger?> FindByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(
                p => p.PhoneNumber.Value == phoneNumber,
                cancellationToken);
    }
}

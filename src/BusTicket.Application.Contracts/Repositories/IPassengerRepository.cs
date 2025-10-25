using BusTicket.Domain.Entities;

namespace BusTicket.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for Passenger entity
/// </summary>
public interface IPassengerRepository
{
    Task<Passenger?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Passenger> AddAsync(Passenger entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Passenger entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Finds passenger by phone number
    /// </summary>
    Task<Passenger?> FindByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds passenger by email
    /// </summary>
    Task<Passenger?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
}

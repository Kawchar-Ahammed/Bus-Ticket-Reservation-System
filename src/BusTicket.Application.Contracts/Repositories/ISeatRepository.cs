using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;

namespace BusTicket.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for Seat entity
/// </summary>
public interface ISeatRepository
{
    Task<Seat?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Seat entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all seats for a specific bus schedule
    /// </summary>
    Task<List<Seat>> GetSeatsByScheduleIdAsync(
        Guid busScheduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available seats for a specific bus schedule
    /// </summary>
    Task<List<Seat>> GetAvailableSeatsAsync(
        Guid busScheduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a seat with bus schedule details
    /// </summary>
    Task<Seat?> GetSeatWithScheduleAsync(
        Guid seatId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a seat is available
    /// </summary>
    Task<bool> IsSeatAvailableAsync(
        Guid seatId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets seats by status
    /// </summary>
    Task<List<Seat>> GetSeatsByStatusAsync(
        Guid busScheduleId,
        SeatStatus status,
        CancellationToken cancellationToken = default);
}

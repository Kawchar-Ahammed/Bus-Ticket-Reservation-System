using BusTicket.Application.Contracts.DTOs;

namespace BusTicket.Application.Contracts.Services;

/// <summary>
/// Application Service Interface for seat booking operations
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Gets the seat plan for a specific bus schedule
    /// </summary>
    /// <param name="busScheduleId">Bus schedule ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Seat plan with all seats and their status</returns>
    Task<SeatPlanDto> GetSeatPlanAsync(
        Guid busScheduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Books a seat for a passenger
    /// </summary>
    /// <param name="input">Booking input data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Booking result with ticket information</returns>
    Task<BookSeatResultDto> BookSeatAsync(
        BookSeatInputDto input,
        CancellationToken cancellationToken = default);
}

using BusTicket.Domain.Entities;

namespace BusTicket.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for Ticket aggregate
/// </summary>
public interface ITicketRepository : IRepository<Ticket>
{
    /// <summary>
    /// Gets ticket by ticket number
    /// </summary>
    Task<Ticket?> GetByTicketNumberAsync(
        string ticketNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tickets for a specific passenger
    /// </summary>
    Task<List<Ticket>> GetTicketsByPassengerIdAsync(
        Guid passengerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tickets for a specific bus schedule
    /// </summary>
    Task<List<Ticket>> GetTicketsByScheduleIdAsync(
        Guid busScheduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets ticket with all related entities
    /// </summary>
    Task<Ticket?> GetTicketWithDetailsAsync(
        Guid ticketId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets ticket by ticket number with all related entities (passenger, bus schedule, seat)
    /// </summary>
    Task<Ticket?> GetByTicketNumberWithDetailsAsync(
        string ticketNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tickets for a phone number with all related entities
    /// </summary>
    Task<List<Ticket>> GetTicketsByPhoneNumberWithDetailsAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets upcoming tickets (not cancelled) within a specific time window
    /// Used for sending journey reminders
    /// </summary>
    Task<IEnumerable<Ticket>> GetUpcomingTicketsWithDetailsAsync(
        DateTime windowStart,
        DateTime windowEnd,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tickets with details for admin view (with optional filters)
    /// </summary>
    Task<List<Ticket>> GetAllTicketsForAdminAsync(
        Guid? companyId = null,
        string? bookingStatus = null,
        string? paymentStatus = null,
        DateTime? journeyDateFrom = null,
        DateTime? journeyDateTo = null,
        DateTime? bookingDateFrom = null,
        DateTime? bookingDateTo = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}

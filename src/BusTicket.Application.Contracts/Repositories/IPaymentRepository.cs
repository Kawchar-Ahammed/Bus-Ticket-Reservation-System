using BusTicket.Domain.Entities;

namespace BusTicket.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for Payment aggregate
/// </summary>
public interface IPaymentRepository : IRepository<Payment>
{
    /// <summary>
    /// Gets payment by ticket ID
    /// </summary>
    Task<Payment?> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payment by transaction ID
    /// </summary>
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets payment with all related data (transactions, ticket)
    /// </summary>
    Task<Payment?> GetPaymentWithDetailsAsync(Guid paymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all payments for a specific ticket with transaction history
    /// </summary>
    Task<List<Payment>> GetPaymentHistoryByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all payments within a date range
    /// </summary>
    Task<List<Payment>> GetPaymentsByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total revenue for a date range
    /// </summary>
    Task<decimal> GetTotalRevenueAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}

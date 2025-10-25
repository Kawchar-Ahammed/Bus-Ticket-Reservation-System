using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using BusTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusTicket.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(BusTicketDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.TicketId == ticketId, cancellationToken);
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);
    }

    public async Task<Payment?> GetPaymentWithDetailsAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Ticket)
                .ThenInclude(t => t.Passenger)
            .Include(p => p.Ticket)
                .ThenInclude(t => t.BusSchedule)
                    .ThenInclude(bs => bs.Route)
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
    }

    public async Task<List<Payment>> GetPaymentHistoryByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Transactions)
            .Where(p => p.TicketId == ticketId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Payment>> GetPaymentsByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Include(p => p.Ticket)
            .Include(p => p.Transactions)
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .Where(p => p.Status == PaymentStatus.Completed)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalRevenueAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var payments = await _context.Payments
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .Where(p => p.Status == PaymentStatus.Completed || p.Status == PaymentStatus.PartiallyRefunded || p.Status == PaymentStatus.Refunded)
            .ToListAsync(cancellationToken);

        return payments.Sum(p => p.Amount.Amount - p.RefundAmount);
    }
}

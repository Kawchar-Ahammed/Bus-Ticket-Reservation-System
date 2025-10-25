using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusTicket.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Ticket aggregate
/// </summary>
public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(BusTicketDbContext context) : base(context)
    {
    }

    public async Task<Ticket?> GetByTicketNumberAsync(
        string ticketNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Bus)
                    .ThenInclude(b => b.Company)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
            .Include(t => t.Passenger)
            .Include(t => t.Seat)
            .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);
    }

    public async Task<Ticket?> GetByBookingReferenceAsync(
        string bookingReference,
        CancellationToken cancellationToken = default)
    {
        // TicketNumber serves as the booking reference
        return await _dbSet
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Bus)
                    .ThenInclude(b => b.Company)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
            .Include(t => t.Passenger)
            .Include(t => t.Seat)
            .FirstOrDefaultAsync(t => t.TicketNumber == bookingReference, cancellationToken);
    }

    public async Task<List<Ticket>> GetTicketsByPassengerIdAsync(
        Guid passengerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Bus)
                    .ThenInclude(b => b.Company)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
            .Include(t => t.Seat)
            .Where(t => t.PassengerId == passengerId)
            .OrderByDescending(t => t.BookingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Ticket>> GetTicketsByScheduleIdAsync(
        Guid scheduleId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Passenger)
            .Include(t => t.Seat)
            .Where(t => t.BusScheduleId == scheduleId)
            .OrderBy(t => t.BookingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Ticket?> GetTicketWithDetailsAsync(
        Guid ticketId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Bus)
                    .ThenInclude(b => b.Company)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.FromLocation)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.ToLocation)
            .Include(t => t.Passenger)
            .Include(t => t.Seat)
            .FirstOrDefaultAsync(t => t.Id == ticketId, cancellationToken);
    }

    public async Task<Ticket?> GetByTicketNumberWithDetailsAsync(
        string ticketNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Bus)
                    .ThenInclude(b => b.Company)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.FromLocation)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.ToLocation)
            .Include(t => t.Passenger)
            .Include(t => t.Seat)
            .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);
    }

    public async Task<List<Ticket>> GetTicketsByPhoneNumberWithDetailsAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Bus)
                    .ThenInclude(b => b.Company)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.FromLocation)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.ToLocation)
            .Include(t => t.Passenger)
            .Include(t => t.Seat)
            .Where(t => t.Passenger.PhoneNumber.Value == phoneNumber)
            .OrderByDescending(t => t.BookingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Ticket>> GetUpcomingTicketsWithDetailsAsync(
        DateTime windowStart,
        DateTime windowEnd,
        CancellationToken cancellationToken = default)
    {
        // We need to filter tickets where journey date + departure time falls within the window
        // Since EF Core doesn't support adding TimeSpan to DateTime in queries, 
        // we'll fetch tickets in the date range and filter by time in memory

        var startDate = windowStart.Date;
        var endDate = windowEnd.Date.AddDays(1); // Include the next day to account for TimeSpan

        var tickets = await _dbSet
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Bus)
                    .ThenInclude(b => b.Company)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.FromLocation)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.ToLocation)
            .Include(t => t.Passenger)
            .Include(t => t.Seat)
            .Where(t => !t.IsCancelled && 
                        t.BusSchedule.JourneyDate >= startDate && 
                        t.BusSchedule.JourneyDate < endDate)
            .ToListAsync(cancellationToken);

        // Filter by exact journey datetime in memory
        return tickets.Where(t =>
        {
            var journeyDateTime = t.BusSchedule.JourneyDate.Add(t.BusSchedule.DepartureTime);
            return journeyDateTime >= windowStart && journeyDateTime <= windowEnd;
        });
    }

    public async Task<List<Ticket>> GetAllTicketsForAdminAsync(
        Guid? companyId = null,
        string? bookingStatus = null,
        string? paymentStatus = null,
        DateTime? journeyDateFrom = null,
        DateTime? journeyDateTo = null,
        DateTime? bookingDateFrom = null,
        DateTime? bookingDateTo = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Bus)
                    .ThenInclude(b => b.Company)
            .Include(t => t.BusSchedule)
                .ThenInclude(s => s.Route)
            .Include(t => t.Passenger)
            .Include(t => t.Seat)
            .AsQueryable();

        // Apply filters
        if (companyId.HasValue)
        {
            query = query.Where(t => t.BusSchedule.Bus.CompanyId == companyId.Value);
        }

        if (!string.IsNullOrEmpty(bookingStatus))
        {
            // Map status string to domain flags
            switch (bookingStatus.ToLower())
            {
                case "confirmed":
                    query = query.Where(t => t.IsConfirmed && !t.IsCancelled);
                    break;
                case "cancelled":
                    query = query.Where(t => t.IsCancelled);
                    break;
                case "pending":
                    query = query.Where(t => !t.IsConfirmed && !t.IsCancelled);
                    break;
            }
        }

        // Note: PaymentStatus filter removed as Ticket doesn't track payment status separately
        // Payment is assumed confirmed when IsConfirmed is true

        if (journeyDateFrom.HasValue)
        {
            query = query.Where(t => t.BusSchedule.JourneyDate >= journeyDateFrom.Value.Date);
        }

        if (journeyDateTo.HasValue)
        {
            query = query.Where(t => t.BusSchedule.JourneyDate <= journeyDateTo.Value.Date);
        }

        if (bookingDateFrom.HasValue)
        {
            query = query.Where(t => t.BookingDate >= bookingDateFrom.Value);
        }

        if (bookingDateTo.HasValue)
        {
            query = query.Where(t => t.BookingDate <= bookingDateTo.Value);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(t =>
                t.TicketNumber.ToLower().Contains(lowerSearchTerm) ||
                t.Passenger.Name.ToLower().Contains(lowerSearchTerm) ||
                t.Passenger.PhoneNumber.Value.Contains(lowerSearchTerm));
        }

        return await query
            .OrderByDescending(t => t.BookingDate)
            .ToListAsync(cancellationToken);
    }
}

using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusTicket.Infrastructure.Repositories;

public class BusRepository : Repository<Bus>, IBusRepository
{
    public BusRepository(BusTicketDbContext context) : base(context)
    {
    }

    public async Task<List<Bus>> GetAllWithCompanyAsync()
    {
        return await _context.Buses
            .Include(b => b.Company)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Bus?> GetByIdWithCompanyAsync(Guid id)
    {
        return await _context.Buses
            .Include(b => b.Company)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Bus?> GetByBusNumberAsync(string busNumber)
    {
        return await _context.Buses
            .FirstOrDefaultAsync(b => b.BusNumber == busNumber);
    }

    public async Task<List<Bus>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.Buses
            .Where(b => b.CompanyId == companyId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }
}

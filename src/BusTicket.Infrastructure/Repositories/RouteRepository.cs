using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusTicket.Infrastructure.Repositories;

public class RouteRepository : Repository<Route>, IRouteRepository
{
    public RouteRepository(BusTicketDbContext context) : base(context)
    {
    }

    public async Task<List<Route>> GetAllAsync()
    {
        return await _context.Routes
            .OrderBy(r => r.FromLocation.City)
            .ThenBy(r => r.ToLocation.City)
            .ToListAsync();
    }

    public async Task<Route?> GetByIdAsync(Guid id)
    {
        return await _context.Routes
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}

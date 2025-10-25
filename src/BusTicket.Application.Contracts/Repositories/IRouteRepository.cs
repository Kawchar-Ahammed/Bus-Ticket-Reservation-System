using BusTicket.Domain.Entities;

namespace BusTicket.Application.Contracts.Repositories;

public interface IRouteRepository : IRepository<Route>
{
    Task<List<Route>> GetAllAsync();
    Task<Route?> GetByIdAsync(Guid id);
}

using BusTicket.Domain.Entities;

namespace BusTicket.Application.Contracts.Repositories;

public interface IBusRepository : IRepository<Bus>
{
    Task<List<Bus>> GetAllWithCompanyAsync();
    Task<Bus?> GetByIdWithCompanyAsync(Guid id);
    Task<Bus?> GetByBusNumberAsync(string busNumber);
    Task<List<Bus>> GetByCompanyIdAsync(Guid companyId);
}

using BusTicket.Domain.Entities;

namespace BusTicket.Application.Contracts.Repositories;

public interface ICompanyRepository : IRepository<Company>
{
    Task<IReadOnlyList<Company>> GetAllAsync();
    Task<Company?> GetByIdAsync(Guid id);
}

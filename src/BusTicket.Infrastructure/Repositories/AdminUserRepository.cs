using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BusTicket.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for AdminUser aggregate
/// </summary>
public class AdminUserRepository : Repository<AdminUser>, IAdminUserRepository
{
    public AdminUserRepository(BusTicketDbContext context) : base(context)
    {
    }

    public async Task<AdminUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.Email == email.ToLower(), cancellationToken);
    }

    public async Task<AdminUser?> GetByRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(a => a.Email == email.ToLower(), cancellationToken);
    }
}

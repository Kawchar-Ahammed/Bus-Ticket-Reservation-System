using BusTicket.Domain.Entities;

namespace BusTicket.Application.Contracts.Repositories;

/// <summary>
/// Repository interface for AdminUser aggregate
/// </summary>
public interface IAdminUserRepository : IRepository<AdminUser>
{
    /// <summary>
    /// Gets admin user by email
    /// </summary>
    Task<AdminUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets admin user by refresh token
    /// </summary>
    Task<AdminUser?> GetByRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if email already exists
    /// </summary>
    Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default);
}

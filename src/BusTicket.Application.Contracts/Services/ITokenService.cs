using BusTicket.Domain.Entities;

namespace BusTicket.Application.Contracts.Services;

/// <summary>
/// Service for generating and managing JWT tokens
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates an access token for an admin user
    /// </summary>
    /// <param name="adminUser">Admin user to generate token for</param>
    /// <returns>JWT access token</returns>
    string GenerateAccessToken(AdminUser adminUser);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <returns>Random refresh token</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Gets the principal from an expired token (for refresh)
    /// </summary>
    /// <param name="token">Expired JWT token</param>
    /// <returns>Claims principal if valid, null otherwise</returns>
    System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

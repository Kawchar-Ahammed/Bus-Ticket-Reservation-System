using BusTicket.Application.Contracts.DTOs.Auth;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Contracts.Services;
using BusTicket.Domain.Exceptions;
using MediatR;
using System.Security.Claims;

namespace BusTicket.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Handler for refresh token command
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponseDto>
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IAdminUserRepository adminUserRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _adminUserRepository = adminUserRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.AccessToken))
            throw new ValidationException("Access token is required");

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new ValidationException("Refresh token is required");

        // Get principal from expired token
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            throw new UnauthorizedException("Invalid access token");

        // Get user email from claims
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
            throw new UnauthorizedException("Invalid token claims");

        // Find admin user by refresh token
        var adminUser = await _adminUserRepository.GetByRefreshTokenAsync(
            request.RefreshToken,
            cancellationToken);

        if (adminUser == null || adminUser.Email != email.ToLower())
            throw new UnauthorizedException("Invalid refresh token");

        // Check if user is active
        if (!adminUser.IsActive)
            throw new UnauthorizedException("Account is deactivated");

        // Validate refresh token
        if (!adminUser.IsRefreshTokenValid(request.RefreshToken))
            throw new UnauthorizedException("Refresh token has expired");

        // Generate new tokens
        var newAccessToken = _tokenService.GenerateAccessToken(adminUser);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Refresh token valid for 7 days
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // Update admin user with new refresh token
        adminUser.SetRefreshToken(newRefreshToken, refreshTokenExpiry);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Access token valid for 60 minutes
        return new TokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}

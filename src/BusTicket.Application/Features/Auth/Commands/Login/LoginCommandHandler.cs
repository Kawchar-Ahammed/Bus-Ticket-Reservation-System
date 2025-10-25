using BusTicket.Application.Contracts.DTOs.Auth;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Contracts.Services;
using BusTicket.Domain.Exceptions;
using MediatR;

namespace BusTicket.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handler for admin login command
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IAdminUserRepository adminUserRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _adminUserRepository = adminUserRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("Password is required");

        // Find admin user by email
        var adminUser = await _adminUserRepository.GetByEmailAsync(
            request.Email.ToLower(),
            cancellationToken);

        if (adminUser == null)
            throw new UnauthorizedException("Invalid email or password");

        // Check if user is active
        if (!adminUser.IsActive)
            throw new UnauthorizedException("Account is deactivated");

        // Verify password
        if (!_passwordHasher.VerifyPassword(request.Password, adminUser.PasswordHash))
            throw new UnauthorizedException("Invalid email or password");

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(adminUser);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Refresh token valid for 7 days
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // Update admin user with refresh token
        adminUser.SetRefreshToken(refreshToken, refreshTokenExpiry);
        adminUser.UpdateLastLogin();

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Access token valid for 60 minutes
        return new LoginResponseDto
        {
            UserId = adminUser.Id,
            Email = adminUser.Email,
            FullName = adminUser.FullName,
            Role = adminUser.Role.ToString(),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}

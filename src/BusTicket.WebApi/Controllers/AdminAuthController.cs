using BusTicket.Application.Contracts.DTOs.Auth;
using BusTicket.Application.Features.Auth.Commands.Login;
using BusTicket.Application.Features.Auth.Commands.Logout;
using BusTicket.Application.Features.Auth.Commands.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusTicket.WebApi.Controllers;

/// <summary>
/// Admin authentication controller
/// </summary>
[ApiController]
[Route("api/admin/auth")]
public class AdminAuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminAuthController> _logger;

    public AdminAuthController(IMediator mediator, ILogger<AdminAuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _mediator.Send(command);

            _logger.LogInformation("Admin user {Email} logged in successfully", request.Email);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Login failed for {Email}", request.Email);
            throw;
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var command = new RefreshTokenCommand
            {
                AccessToken = request.AccessToken,
                RefreshToken = request.RefreshToken
            };

            var result = await _mediator.Send(command);

            _logger.LogInformation("Token refreshed successfully");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token refresh failed");
            throw;
        }
    }

    /// <summary>
    /// Logout current user
    /// </summary>
    [HttpPost("logout")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public async Task<ActionResult> Logout()
    {
        try
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid token");
            }

            var command = new LogoutCommand { UserId = userId };
            await _mediator.Send(command);

            _logger.LogInformation("Admin user {UserId} logged out successfully", userId);

            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Logout failed");
            throw;
        }
    }

    /// <summary>
    /// Get current user info
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "SuperAdmin,Admin,Operator")]
    public ActionResult<object> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            userId,
            email,
            name,
            role
        });
    }
}

using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Exceptions;
using MediatR;

namespace BusTicket.Application.Features.AdminUsers.Queries.GetAdminUserById;

/// <summary>
/// Query to get admin user by ID
/// </summary>
public record GetAdminUserByIdQuery(Guid Id) : IRequest<AdminUserDto>;

public class GetAdminUserByIdQueryHandler : IRequestHandler<GetAdminUserByIdQuery, AdminUserDto>
{
    private readonly IAdminUserRepository _adminUserRepository;

    public GetAdminUserByIdQueryHandler(IAdminUserRepository adminUserRepository)
    {
        _adminUserRepository = adminUserRepository;
    }

    public async Task<AdminUserDto> Handle(
        GetAdminUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var adminUser = await _adminUserRepository.GetByIdAsync(request.Id);
        
        if (adminUser == null)
        {
            throw new NotFoundException($"Admin user with ID {request.Id} not found");
        }

        return new AdminUserDto
        {
            Id = adminUser.Id,
            Email = adminUser.Email,
            FullName = adminUser.FullName,
            Role = adminUser.Role,
            RoleName = adminUser.Role.ToString(),
            IsActive = adminUser.IsActive,
            LastLoginDate = adminUser.LastLoginDate,
            CreatedAt = adminUser.CreatedAt,
            UpdatedAt = adminUser.UpdatedAt
        };
    }
}

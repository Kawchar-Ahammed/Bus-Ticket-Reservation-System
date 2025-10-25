using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Enums;
using MediatR;

namespace BusTicket.Application.Features.AdminUsers.Queries.GetAllAdminUsers;

/// <summary>
/// Query to get all admin users
/// </summary>
public record GetAllAdminUsersQuery(
    AdminRole? Role = null,
    bool? IsActive = null
) : IRequest<List<AdminUserDto>>;

public class GetAllAdminUsersQueryHandler : IRequestHandler<GetAllAdminUsersQuery, List<AdminUserDto>>
{
    private readonly IAdminUserRepository _adminUserRepository;

    public GetAllAdminUsersQueryHandler(IAdminUserRepository adminUserRepository)
    {
        _adminUserRepository = adminUserRepository;
    }

    public async Task<List<AdminUserDto>> Handle(
        GetAllAdminUsersQuery request,
        CancellationToken cancellationToken)
    {
        var allUsers = await _adminUserRepository.GetAllAsync();
        
        var filteredUsers = allUsers.AsEnumerable();

        // Apply filters
        if (request.Role.HasValue)
        {
            filteredUsers = filteredUsers.Where(u => u.Role == request.Role.Value);
        }

        if (request.IsActive.HasValue)
        {
            filteredUsers = filteredUsers.Where(u => u.IsActive == request.IsActive.Value);
        }

        var adminUsers = filteredUsers
            .OrderBy(u => u.Role)
            .ThenBy(u => u.FullName)
            .ToList();

        return adminUsers.Select(u => new AdminUserDto
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            Role = u.Role,
            RoleName = u.Role.ToString(),
            IsActive = u.IsActive,
            LastLoginDate = u.LastLoginDate,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();
    }
}

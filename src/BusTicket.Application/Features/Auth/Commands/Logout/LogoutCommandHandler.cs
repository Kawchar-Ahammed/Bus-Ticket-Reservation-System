using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Exceptions;
using MediatR;

namespace BusTicket.Application.Features.Auth.Commands.Logout;

/// <summary>
/// Handler for logout command
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(
        IAdminUserRepository adminUserRepository,
        IUnitOfWork unitOfWork)
    {
        _adminUserRepository = adminUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // Find admin user
        var adminUser = await _adminUserRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (adminUser == null)
            throw new NotFoundException("Admin user not found");

        // Clear refresh token
        adminUser.ClearRefreshToken();

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

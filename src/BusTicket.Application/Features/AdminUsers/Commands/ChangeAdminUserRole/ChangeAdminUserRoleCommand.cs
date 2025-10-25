using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Enums;
using BusTicket.Domain.Exceptions;
using FluentValidation;
using MediatR;
using DomainValidationException = BusTicket.Domain.Exceptions.ValidationException;

namespace BusTicket.Application.Features.AdminUsers.Commands.ChangeAdminUserRole;

/// <summary>
/// Command to change admin user role
/// </summary>
public record ChangeAdminUserRoleCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public AdminRole NewRole { get; init; }
}

public class ChangeAdminUserRoleCommandValidator : AbstractValidator<ChangeAdminUserRoleCommand>
{
    public ChangeAdminUserRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.NewRole)
            .IsInEnum().WithMessage("Invalid role");
    }
}

public class ChangeAdminUserRoleCommandHandler : IRequestHandler<ChangeAdminUserRoleCommand, Unit>
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ChangeAdminUserRoleCommand> _validator;

    public ChangeAdminUserRoleCommandHandler(
        IAdminUserRepository adminUserRepository,
        IUnitOfWork unitOfWork,
        IValidator<ChangeAdminUserRoleCommand> validator)
    {
        _adminUserRepository = adminUserRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Unit> Handle(
        ChangeAdminUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        var adminUser = await _adminUserRepository.GetByIdAsync(request.Id);
        if (adminUser == null)
        {
            throw new NotFoundException($"Admin user with ID {request.Id} not found");
        }

        // If downgrading from SuperAdmin, ensure at least one SuperAdmin remains
        if (adminUser.Role == AdminRole.SuperAdmin && request.NewRole != AdminRole.SuperAdmin)
        {
            var allUsers = await _adminUserRepository.GetAllAsync();
            var superAdminCount = allUsers
                .Count(u => u.Role == AdminRole.SuperAdmin && u.IsActive);

            if (superAdminCount <= 1)
            {
                throw new BusinessRuleViolationException(
                    "Cannot change role. At least one active SuperAdmin must remain in the system");
            }
        }

        adminUser.UpdateRole(request.NewRole);

        await _adminUserRepository.UpdateAsync(adminUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

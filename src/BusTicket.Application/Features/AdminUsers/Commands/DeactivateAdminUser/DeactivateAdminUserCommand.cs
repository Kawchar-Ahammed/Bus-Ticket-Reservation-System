using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Enums;
using BusTicket.Domain.Exceptions;
using FluentValidation;
using MediatR;
using DomainValidationException = BusTicket.Domain.Exceptions.ValidationException;

namespace BusTicket.Application.Features.AdminUsers.Commands.DeactivateAdminUser;

/// <summary>
/// Command to deactivate admin user
/// </summary>
public record DeactivateAdminUserCommand(Guid Id) : IRequest<Unit>;

public class DeactivateAdminUserCommandValidator : AbstractValidator<DeactivateAdminUserCommand>
{
    public DeactivateAdminUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required");
    }
}

public class DeactivateAdminUserCommandHandler : IRequestHandler<DeactivateAdminUserCommand, Unit>
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeactivateAdminUserCommand> _validator;

    public DeactivateAdminUserCommandHandler(
        IAdminUserRepository adminUserRepository,
        IUnitOfWork unitOfWork,
        IValidator<DeactivateAdminUserCommand> validator)
    {
        _adminUserRepository = adminUserRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Unit> Handle(
        DeactivateAdminUserCommand request,
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

        // If deactivating a SuperAdmin, ensure at least one active SuperAdmin remains
        if (adminUser.Role == AdminRole.SuperAdmin)
        {
            var allUsers = await _adminUserRepository.GetAllAsync();
            var activeSuperAdminCount = allUsers
                .Count(u => u.Role == AdminRole.SuperAdmin && u.IsActive && u.Id != request.Id);

            if (activeSuperAdminCount == 0)
            {
                throw new BusinessRuleViolationException(
                    "Cannot deactivate user. At least one active SuperAdmin must remain in the system");
            }
        }

        adminUser.Deactivate();

        await _adminUserRepository.UpdateAsync(adminUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Exceptions;
using FluentValidation;
using MediatR;
using DomainValidationException = BusTicket.Domain.Exceptions.ValidationException;

namespace BusTicket.Application.Features.AdminUsers.Commands.ActivateAdminUser;

/// <summary>
/// Command to activate admin user
/// </summary>
public record ActivateAdminUserCommand(Guid Id) : IRequest<Unit>;

public class ActivateAdminUserCommandValidator : AbstractValidator<ActivateAdminUserCommand>
{
    public ActivateAdminUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required");
    }
}

public class ActivateAdminUserCommandHandler : IRequestHandler<ActivateAdminUserCommand, Unit>
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ActivateAdminUserCommand> _validator;

    public ActivateAdminUserCommandHandler(
        IAdminUserRepository adminUserRepository,
        IUnitOfWork unitOfWork,
        IValidator<ActivateAdminUserCommand> validator)
    {
        _adminUserRepository = adminUserRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Unit> Handle(
        ActivateAdminUserCommand request,
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

        adminUser.Activate();

        await _adminUserRepository.UpdateAsync(adminUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

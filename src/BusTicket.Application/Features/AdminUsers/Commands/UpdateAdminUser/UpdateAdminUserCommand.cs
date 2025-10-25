using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Exceptions;
using FluentValidation;
using MediatR;
using DomainValidationException = BusTicket.Domain.Exceptions.ValidationException;

namespace BusTicket.Application.Features.AdminUsers.Commands.UpdateAdminUser;

/// <summary>
/// Command to update admin user profile
/// </summary>
public record UpdateAdminUserCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = null!;
}

public class UpdateAdminUserCommandValidator : AbstractValidator<UpdateAdminUserCommand>
{
    public UpdateAdminUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");
    }
}

public class UpdateAdminUserCommandHandler : IRequestHandler<UpdateAdminUserCommand, Unit>
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateAdminUserCommand> _validator;

    public UpdateAdminUserCommandHandler(
        IAdminUserRepository adminUserRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateAdminUserCommand> validator)
    {
        _adminUserRepository = adminUserRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Unit> Handle(
        UpdateAdminUserCommand request,
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

        adminUser.UpdateProfile(request.FullName);

        await _adminUserRepository.UpdateAsync(adminUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

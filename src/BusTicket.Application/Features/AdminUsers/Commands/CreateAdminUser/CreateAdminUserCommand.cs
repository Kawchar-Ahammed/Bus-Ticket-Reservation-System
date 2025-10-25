using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Contracts.Services;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using BusTicket.Domain.Exceptions;
using FluentValidation;
using MediatR;
using DomainValidationException = BusTicket.Domain.Exceptions.ValidationException;

namespace BusTicket.Application.Features.AdminUsers.Commands.CreateAdminUser;

/// <summary>
/// Command to create a new admin user
/// </summary>
public record CreateAdminUserCommand : IRequest<Guid>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public AdminRole Role { get; init; }
}

public class CreateAdminUserCommandValidator : AbstractValidator<CreateAdminUserCommand>
{
    public CreateAdminUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role");
    }
}

public class CreateAdminUserCommandHandler : IRequestHandler<CreateAdminUserCommand, Guid>
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateAdminUserCommand> _validator;

    public CreateAdminUserCommandHandler(
        IAdminUserRepository adminUserRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        IValidator<CreateAdminUserCommand> validator)
    {
        _adminUserRepository = adminUserRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Guid> Handle(
        CreateAdminUserCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        // Check if email already exists
        var emailExists = await _adminUserRepository.EmailExistsAsync(
            request.Email,
            cancellationToken);

        if (emailExists)
        {
            throw new BusinessRuleViolationException("An admin user with this email already exists");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create admin user
        var adminUser = AdminUser.Create(
            request.Email,
            passwordHash,
            request.FullName,
            request.Role);

        await _adminUserRepository.AddAsync(adminUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return adminUser.Id;
    }
}

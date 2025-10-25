using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Exceptions;
using FluentValidation;
using MediatR;
using DomainValidationException = BusTicket.Domain.Exceptions.ValidationException;

namespace BusTicket.Application.Features.Buses.Commands;

public record CreateBusCommand : IRequest<Guid>
{
    public string BusNumber { get; init; } = string.Empty;
    public string BusName { get; init; } = string.Empty;
    public Guid CompanyId { get; init; }
    public int TotalSeats { get; init; }
    public string? Description { get; init; }
}

public class CreateBusCommandValidator : AbstractValidator<CreateBusCommand>
{
    public CreateBusCommandValidator()
    {
        RuleFor(x => x.BusNumber)
            .NotEmpty().WithMessage("Bus number is required")
            .MaximumLength(50).WithMessage("Bus number must not exceed 50 characters");

        RuleFor(x => x.BusName)
            .NotEmpty().WithMessage("Bus name is required")
            .MaximumLength(200).WithMessage("Bus name must not exceed 200 characters");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company is required");

        RuleFor(x => x.TotalSeats)
            .GreaterThan(0).WithMessage("Total seats must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Total seats must not exceed 100");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
    }
}

public class CreateBusCommandHandler : IRequestHandler<CreateBusCommand, Guid>
{
    private readonly IBusRepository _busRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IValidator<CreateBusCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBusCommandHandler(
        IBusRepository busRepository,
        ICompanyRepository companyRepository,
        IValidator<CreateBusCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _busRepository = busRepository;
        _companyRepository = companyRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateBusCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        // Check if company exists
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);
        if (company == null)
        {
            throw new NotFoundException($"Company with ID {request.CompanyId} not found");
        }

        // Check if bus number already exists
        var existingBus = await _busRepository.GetByBusNumberAsync(request.BusNumber);
        if (existingBus != null)
        {
            throw new DomainValidationException($"Bus number '{request.BusNumber}' already exists");
        }

        var bus = Bus.Create(
            request.BusNumber,
            request.BusName,
            request.CompanyId,
            request.TotalSeats,
            request.Description
        );

        await _busRepository.AddAsync(bus);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return bus.Id;
    }
}

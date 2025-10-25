using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Exceptions;
using FluentValidation;
using MediatR;
using DomainValidationException = BusTicket.Domain.Exceptions.ValidationException;

namespace BusTicket.Application.Features.Buses.Commands;

public record UpdateBusCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string BusName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}

public class UpdateBusCommandValidator : AbstractValidator<UpdateBusCommand>
{
    public UpdateBusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Bus ID is required");

        RuleFor(x => x.BusName)
            .NotEmpty().WithMessage("Bus name is required")
            .MaximumLength(200).WithMessage("Bus name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
    }
}

public class UpdateBusCommandHandler : IRequestHandler<UpdateBusCommand, Unit>
{
    private readonly IBusRepository _busRepository;
    private readonly IValidator<UpdateBusCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBusCommandHandler(
        IBusRepository busRepository,
        IValidator<UpdateBusCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _busRepository = busRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateBusCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        var bus = await _busRepository.GetByIdAsync(request.Id);
        if (bus == null)
        {
            throw new NotFoundException($"Bus with ID {request.Id} not found");
        }

        bus.UpdateDetails(request.BusName, request.Description);

        if (request.IsActive && !bus.IsActive)
        {
            bus.Activate();
        }
        else if (!request.IsActive && bus.IsActive)
        {
            bus.Deactivate();
        }

        await _busRepository.UpdateAsync(bus);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

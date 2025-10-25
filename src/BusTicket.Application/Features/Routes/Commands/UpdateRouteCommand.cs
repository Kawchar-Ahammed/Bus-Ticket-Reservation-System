using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Exceptions;
using FluentValidation;
using MediatR;
using DomainValidationException = BusTicket.Domain.Exceptions.ValidationException;

namespace BusTicket.Application.Features.Routes.Commands;

public record UpdateRouteCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public decimal DistanceInKm { get; init; }
    public int DurationHours { get; init; }
    public int DurationMinutes { get; init; }
    public bool IsActive { get; init; }
}

public class UpdateRouteCommandValidator : AbstractValidator<UpdateRouteCommand>
{
    public UpdateRouteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Route ID is required");

        RuleFor(x => x.DistanceInKm)
            .GreaterThan(0).WithMessage("Distance must be greater than 0");

        RuleFor(x => x.DurationHours)
            .GreaterThanOrEqualTo(0).WithMessage("Duration hours cannot be negative")
            .LessThan(72).WithMessage("Duration hours must be less than 72");

        RuleFor(x => x.DurationMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Duration minutes cannot be negative")
            .LessThan(60).WithMessage("Duration minutes must be less than 60");

        RuleFor(x => x)
            .Must(x => x.DurationHours > 0 || x.DurationMinutes > 0)
            .WithMessage("Total duration must be greater than 0");
    }
}

public class UpdateRouteCommandHandler : IRequestHandler<UpdateRouteCommand, Unit>
{
    private readonly IRouteRepository _routeRepository;
    private readonly IValidator<UpdateRouteCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRouteCommandHandler(
        IRouteRepository routeRepository,
        IValidator<UpdateRouteCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _routeRepository = routeRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        var route = await _routeRepository.GetByIdAsync(request.Id);
        if (route == null)
        {
            throw new NotFoundException($"Route with ID {request.Id} not found");
        }

        var duration = new TimeSpan(request.DurationHours, request.DurationMinutes, 0);
        route.UpdateDetails(request.DistanceInKm, duration);

        if (request.IsActive && !route.IsActive)
        {
            route.Activate();
        }
        else if (!request.IsActive && route.IsActive)
        {
            route.Deactivate();
        }

        await _routeRepository.UpdateAsync(route);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

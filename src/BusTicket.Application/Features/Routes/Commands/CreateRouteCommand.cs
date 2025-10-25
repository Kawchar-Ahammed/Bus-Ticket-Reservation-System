using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Exceptions;
using BusTicket.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using DomainValidationException = BusTicket.Domain.Exceptions.ValidationException;

namespace BusTicket.Application.Features.Routes.Commands;

public record CreateRouteCommand : IRequest<Guid>
{
    public string FromCity { get; init; } = string.Empty;
    public string ToCity { get; init; } = string.Empty;
    public decimal DistanceInKm { get; init; }
    public int DurationHours { get; init; }
    public int DurationMinutes { get; init; }
}

public class CreateRouteCommandValidator : AbstractValidator<CreateRouteCommand>
{
    public CreateRouteCommandValidator()
    {
        RuleFor(x => x.FromCity)
            .NotEmpty().WithMessage("From city is required")
            .MaximumLength(100).WithMessage("From city must not exceed 100 characters");

        RuleFor(x => x.ToCity)
            .NotEmpty().WithMessage("To city is required")
            .MaximumLength(100).WithMessage("To city must not exceed 100 characters")
            .NotEqual(x => x.FromCity).WithMessage("From and To cities cannot be the same");

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

public class CreateRouteCommandHandler : IRequestHandler<CreateRouteCommand, Guid>
{
    private readonly IRouteRepository _routeRepository;
    private readonly IValidator<CreateRouteCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRouteCommandHandler(
        IRouteRepository routeRepository,
        IValidator<CreateRouteCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _routeRepository = routeRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        var fromAddress = Address.Create(request.FromCity);
        var toAddress = Address.Create(request.ToCity);
        var duration = new TimeSpan(request.DurationHours, request.DurationMinutes, 0);

        var route = Route.Create(fromAddress, toAddress, request.DistanceInKm, duration);

        await _routeRepository.AddAsync(route);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return route.Id;
    }
}

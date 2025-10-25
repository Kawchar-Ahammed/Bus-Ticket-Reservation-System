using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Exceptions;
using BusTicket.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using DomainValidationException = BusTicket.Domain.Exceptions.ValidationException;

namespace BusTicket.Application.Features.BusSchedules.Commands;

public record CreateBusScheduleCommand : IRequest<Guid>
{
    public Guid BusId { get; init; }
    public Guid RouteId { get; init; }
    public DateTime JourneyDate { get; init; }
    public int DepartureHour { get; init; }
    public int DepartureMinute { get; init; }
    public int ArrivalHour { get; init; }
    public int ArrivalMinute { get; init; }
    public decimal FareAmount { get; init; }
    public string FareCurrency { get; init; } = "BDT";
    public string BoardingCity { get; init; } = string.Empty;
    public string DroppingCity { get; init; } = string.Empty;
}

public class CreateBusScheduleCommandValidator : AbstractValidator<CreateBusScheduleCommand>
{
    public CreateBusScheduleCommandValidator()
    {
        RuleFor(x => x.BusId)
            .NotEmpty().WithMessage("Bus is required");

        RuleFor(x => x.RouteId)
            .NotEmpty().WithMessage("Route is required");

        RuleFor(x => x.JourneyDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Journey date cannot be in the past");

        RuleFor(x => x.DepartureHour)
            .InclusiveBetween(0, 23).WithMessage("Departure hour must be between 0 and 23");

        RuleFor(x => x.DepartureMinute)
            .InclusiveBetween(0, 59).WithMessage("Departure minute must be between 0 and 59");

        RuleFor(x => x.ArrivalHour)
            .InclusiveBetween(0, 23).WithMessage("Arrival hour must be between 0 and 23");

        RuleFor(x => x.ArrivalMinute)
            .InclusiveBetween(0, 59).WithMessage("Arrival minute must be between 0 and 59");

        RuleFor(x => x.FareAmount)
            .GreaterThan(0).WithMessage("Fare amount must be greater than 0");

        RuleFor(x => x.FareCurrency)
            .NotEmpty().WithMessage("Currency is required")
            .MaximumLength(3).WithMessage("Currency code must be 3 characters");

        RuleFor(x => x.BoardingCity)
            .NotEmpty().WithMessage("Boarding city is required")
            .MaximumLength(100).WithMessage("Boarding city must not exceed 100 characters");

        RuleFor(x => x.DroppingCity)
            .NotEmpty().WithMessage("Dropping city is required")
            .MaximumLength(100).WithMessage("Dropping city must not exceed 100 characters");
    }
}

public class CreateBusScheduleCommandHandler : IRequestHandler<CreateBusScheduleCommand, Guid>
{
    private readonly IBusScheduleRepository _busScheduleRepository;
    private readonly IBusRepository _busRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IValidator<CreateBusScheduleCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBusScheduleCommandHandler(
        IBusScheduleRepository busScheduleRepository,
        IBusRepository busRepository,
        IRouteRepository routeRepository,
        IValidator<CreateBusScheduleCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _busScheduleRepository = busScheduleRepository;
        _busRepository = busRepository;
        _routeRepository = routeRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateBusScheduleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        // Verify bus exists and is active
        var bus = await _busRepository.GetByIdAsync(request.BusId);
        if (bus == null)
        {
            throw new NotFoundException($"Bus with ID {request.BusId} not found");
        }

        if (!bus.IsActive)
        {
            throw new DomainValidationException("Cannot create schedule for inactive bus");
        }

        // Verify route exists and is active
        var route = await _routeRepository.GetByIdAsync(request.RouteId);
        if (route == null)
        {
            throw new NotFoundException($"Route with ID {request.RouteId} not found");
        }

        if (!route.IsActive)
        {
            throw new DomainValidationException("Cannot create schedule for inactive route");
        }

        var departureTime = new TimeSpan(request.DepartureHour, request.DepartureMinute, 0);
        var arrivalTime = new TimeSpan(request.ArrivalHour, request.ArrivalMinute, 0);
        var fare = Money.Create(request.FareAmount, request.FareCurrency);
        var boardingPoint = Address.Create(request.BoardingCity);
        var droppingPoint = Address.Create(request.DroppingCity);

        var schedule = BusSchedule.Create(
            request.BusId,
            request.RouteId,
            request.JourneyDate,
            departureTime,
            arrivalTime,
            fare,
            boardingPoint,
            droppingPoint);

        await _busScheduleRepository.AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return schedule.Id;
    }
}

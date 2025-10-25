using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Exceptions;
using BusTicket.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using DomainValidationException = BusTicket.Domain.Exceptions.ValidationException;

namespace BusTicket.Application.Features.BusSchedules.Commands;

public record UpdateBusScheduleCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public DateTime JourneyDate { get; init; }
    public int DepartureHour { get; init; }
    public int DepartureMinute { get; init; }
    public int ArrivalHour { get; init; }
    public int ArrivalMinute { get; init; }
    public decimal FareAmount { get; init; }
    public string FareCurrency { get; init; } = "BDT";
    public string BoardingCity { get; init; } = string.Empty;
    public string DroppingCity { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

public class UpdateBusScheduleCommandValidator : AbstractValidator<UpdateBusScheduleCommand>
{
    public UpdateBusScheduleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Schedule ID is required");

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

        RuleFor(x => x.BoardingCity)
            .NotEmpty().WithMessage("Boarding city is required")
            .MaximumLength(100).WithMessage("Boarding city must not exceed 100 characters");

        RuleFor(x => x.DroppingCity)
            .NotEmpty().WithMessage("Dropping city is required")
            .MaximumLength(100).WithMessage("Dropping city must not exceed 100 characters");
    }
}

public class UpdateBusScheduleCommandHandler : IRequestHandler<UpdateBusScheduleCommand, Unit>
{
    private readonly IBusScheduleRepository _busScheduleRepository;
    private readonly IValidator<UpdateBusScheduleCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBusScheduleCommandHandler(
        IBusScheduleRepository busScheduleRepository,
        IValidator<UpdateBusScheduleCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _busScheduleRepository = busScheduleRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateBusScheduleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainValidationException(errors);
        }

        var schedule = await _busScheduleRepository.GetByIdAsync(request.Id);
        if (schedule == null)
        {
            throw new NotFoundException($"Bus schedule with ID {request.Id} not found");
        }

        var departureTime = new TimeSpan(request.DepartureHour, request.DepartureMinute, 0);
        var arrivalTime = new TimeSpan(request.ArrivalHour, request.ArrivalMinute, 0);
        var fare = Money.Create(request.FareAmount, request.FareCurrency);

        schedule.UpdateSchedule(request.JourneyDate, departureTime, arrivalTime, fare);

        var boardingPoint = Address.Create(request.BoardingCity);
        var droppingPoint = Address.Create(request.DroppingCity);
        schedule.UpdateBoardingAndDroppingPoints(boardingPoint, droppingPoint);

        if (!request.IsActive && schedule.IsActive)
        {
            schedule.Cancel();
        }

        await _busScheduleRepository.UpdateAsync(schedule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

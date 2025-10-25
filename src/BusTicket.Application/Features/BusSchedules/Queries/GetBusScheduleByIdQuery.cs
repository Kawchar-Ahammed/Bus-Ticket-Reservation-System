using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.DTOs;
using MediatR;

namespace BusTicket.Application.Features.BusSchedules.Queries;

public record GetBusScheduleByIdQuery(Guid Id) : IRequest<BusScheduleDetailDto?>;

public class GetBusScheduleByIdQueryHandler : IRequestHandler<GetBusScheduleByIdQuery, BusScheduleDetailDto?>
{
    private readonly IBusScheduleRepository _busScheduleRepository;

    public GetBusScheduleByIdQueryHandler(IBusScheduleRepository busScheduleRepository)
    {
        _busScheduleRepository = busScheduleRepository;
    }

    public async Task<BusScheduleDetailDto?> Handle(GetBusScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _busScheduleRepository.GetByIdWithDetailsAsync(request.Id);

        if (schedule == null)
            return null;

        return new BusScheduleDetailDto
        {
            Id = schedule.Id,
            BusId = schedule.BusId,
            BusNumber = schedule.Bus.BusNumber,
            BusName = schedule.Bus.BusName,
            CompanyName = schedule.Bus.Company.Name,
            RouteId = schedule.RouteId,
            FromCity = schedule.Route.FromLocation.City,
            ToCity = schedule.Route.ToLocation.City,
            RouteDistance = schedule.Route.DistanceInKm,
            JourneyDate = schedule.JourneyDate,
            DepartureTime = schedule.DepartureTime,
            ArrivalTime = schedule.ArrivalTime,
            FareAmount = schedule.Fare.Amount,
            FareCurrency = schedule.Fare.Currency,
            BoardingCity = schedule.BoardingPoint.City,
            DroppingCity = schedule.DroppingPoint.City,
            IsActive = schedule.IsActive,
            AvailableSeats = schedule.GetAvailableSeatsCount(),
            TotalSeats = schedule.Bus.TotalSeats,
            CreatedAt = schedule.CreatedAt,
            UpdatedAt = schedule.UpdatedAt,
            CreatedBy = schedule.CreatedBy,
            UpdatedBy = schedule.UpdatedBy
        };
    }
}

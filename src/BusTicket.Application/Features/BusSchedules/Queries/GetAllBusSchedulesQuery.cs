using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.DTOs;
using MediatR;

namespace BusTicket.Application.Features.BusSchedules.Queries;

public record GetAllBusSchedulesQuery : IRequest<List<BusScheduleDto>>;

public class GetAllBusSchedulesQueryHandler : IRequestHandler<GetAllBusSchedulesQuery, List<BusScheduleDto>>
{
    private readonly IBusScheduleRepository _busScheduleRepository;

    public GetAllBusSchedulesQueryHandler(IBusScheduleRepository busScheduleRepository)
    {
        _busScheduleRepository = busScheduleRepository;
    }

    public async Task<List<BusScheduleDto>> Handle(GetAllBusSchedulesQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _busScheduleRepository.GetAllWithDetailsAsync();

        return schedules.Select(schedule => new BusScheduleDto
        {
            Id = schedule.Id,
            BusId = schedule.BusId,
            BusNumber = schedule.Bus.BusNumber,
            BusName = schedule.Bus.BusName,
            RouteId = schedule.RouteId,
            FromCity = schedule.Route.FromLocation.City,
            ToCity = schedule.Route.ToLocation.City,
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
            CreatedAt = schedule.CreatedAt
        }).ToList();
    }
}

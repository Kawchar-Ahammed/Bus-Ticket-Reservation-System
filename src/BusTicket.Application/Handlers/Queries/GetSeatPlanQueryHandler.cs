using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Queries;
using BusTicket.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicket.Application.Handlers.Queries;

/// <summary>
/// Handler for GetSeatPlanQuery
/// </summary>
public class GetSeatPlanQueryHandler : IRequestHandler<GetSeatPlanQuery, SeatPlanDto>
{
    private readonly IBusScheduleRepository _busScheduleRepository;
    private readonly ISeatRepository _seatRepository;
    private readonly ILogger<GetSeatPlanQueryHandler> _logger;

    public GetSeatPlanQueryHandler(
        IBusScheduleRepository busScheduleRepository,
        ISeatRepository seatRepository,
        ILogger<GetSeatPlanQueryHandler> logger)
    {
        _busScheduleRepository = busScheduleRepository;
        _seatRepository = seatRepository;
        _logger = logger;
    }

    public async Task<SeatPlanDto> Handle(
        GetSeatPlanQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting seat plan for bus schedule {BusScheduleId}", request.BusScheduleId);

        // Get bus schedule with details
        var busSchedule = await _busScheduleRepository.GetWithDetailsAsync(
            request.BusScheduleId,
            cancellationToken);

        if (busSchedule == null)
        {
            _logger.LogWarning("Bus schedule {BusScheduleId} not found", request.BusScheduleId);
            throw new EntityNotFoundException(nameof(busSchedule), request.BusScheduleId);
        }

        // Get seats for this schedule
        var seats = await _seatRepository.GetSeatsByScheduleIdAsync(
            request.BusScheduleId,
            cancellationToken);

        if (!seats.Any())
        {
            _logger.LogWarning("No seats found for bus schedule {BusScheduleId}", request.BusScheduleId);
        }

        // Map to DTO
        var seatPlanDto = new SeatPlanDto
        {
            BusScheduleId = busSchedule.Id,
            BusName = busSchedule.Bus.BusName,
            CompanyName = busSchedule.Bus.Company.Name,
            JourneyDate = busSchedule.JourneyDate,
            DepartureTime = busSchedule.DepartureTime,
            FromCity = busSchedule.Route.FromLocation.City,
            ToCity = busSchedule.Route.ToLocation.City,
            Fare = busSchedule.Fare.Amount,
            Currency = busSchedule.Fare.Currency,
            TotalSeats = busSchedule.Bus.TotalSeats,
            AvailableSeats = busSchedule.GetAvailableSeatsCount(),
            BookedSeats = busSchedule.GetBookedSeatsCount(),
            Seats = seats.Select(seat => new SeatDto
            {
                SeatId = seat.Id,
                SeatNumber = seat.SeatNumber,
                Row = seat.Row,
                Column = seat.Column,
                Status = seat.Status.ToString()
            })
            .OrderBy(s => s.Row)
            .ThenBy(s => s.Column)
            .ToList()
        };

        _logger.LogInformation(
            "Retrieved seat plan with {TotalSeats} seats ({Available} available)",
            seatPlanDto.TotalSeats,
            seatPlanDto.AvailableSeats);

        return seatPlanDto;
    }
}

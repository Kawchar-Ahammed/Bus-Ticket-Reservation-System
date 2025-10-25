using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Queries;
using BusTicket.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicket.Application.Handlers.Queries;

/// <summary>
/// Handler for SearchAvailableBusesQuery
/// </summary>
public class SearchAvailableBusesQueryHandler : IRequestHandler<SearchAvailableBusesQuery, List<AvailableBusDto>>
{
    private readonly IBusScheduleRepository _busScheduleRepository;
    private readonly ILogger<SearchAvailableBusesQueryHandler> _logger;

    public SearchAvailableBusesQueryHandler(
        IBusScheduleRepository busScheduleRepository,
        ILogger<SearchAvailableBusesQueryHandler> logger)
    {
        _busScheduleRepository = busScheduleRepository;
        _logger = logger;
    }

    public async Task<List<AvailableBusDto>> Handle(
        SearchAvailableBusesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Searching for buses from {From} to {To} on {Date}",
            request.From,
            request.To,
            request.JourneyDate);

        // Find bus schedules matching the route and date
        var busSchedules = await _busScheduleRepository.FindByRouteAndDateAsync(
            request.From,
            request.To,
            request.JourneyDate,
            cancellationToken);

        if (!busSchedules.Any())
        {
            _logger.LogInformation("No buses found for the search criteria");
            return new List<AvailableBusDto>();
        }

        // Map to DTOs
        var result = busSchedules
            .Where(bs => bs.IsActive)
            .Select(bs => new AvailableBusDto
            {
                BusScheduleId = bs.Id,
                BusId = bs.BusId,
                CompanyName = bs.Bus.Company.Name,
                BusName = bs.Bus.BusName,
                BusNumber = bs.Bus.BusNumber,
                JourneyDate = bs.JourneyDate,
                DepartureTime = bs.DepartureTime,
                ArrivalTime = bs.ArrivalTime,
                FromCity = bs.Route.FromLocation.City,
                ToCity = bs.Route.ToLocation.City,
                TotalSeats = bs.Bus.TotalSeats,
                BookedSeats = bs.GetBookedSeatsCount(),
                SeatsLeft = bs.GetAvailableSeatsCount(),
                Fare = bs.Fare.Amount,
                Currency = bs.Fare.Currency,
                BoardingPoint = bs.BoardingPoint.ToString(),
                DroppingPoint = bs.DroppingPoint.ToString()
            })
            .OrderBy(b => b.DepartureTime)
            .ToList();

        _logger.LogInformation("Found {Count} buses", result.Count);

        return result;
    }
}

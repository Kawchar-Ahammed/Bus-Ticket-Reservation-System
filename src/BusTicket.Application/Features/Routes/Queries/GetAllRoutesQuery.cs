using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.DTOs;
using MediatR;

namespace BusTicket.Application.Features.Routes.Queries;

public record GetAllRoutesQuery : IRequest<List<RouteDto>>;

public class GetAllRoutesQueryHandler : IRequestHandler<GetAllRoutesQuery, List<RouteDto>>
{
    private readonly IRouteRepository _routeRepository;

    public GetAllRoutesQueryHandler(IRouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public async Task<List<RouteDto>> Handle(GetAllRoutesQuery request, CancellationToken cancellationToken)
    {
        var routes = await _routeRepository.GetAllAsync();

        return routes.Select(route => new RouteDto
        {
            Id = route.Id,
            FromCity = route.FromLocation.City,
            ToCity = route.ToLocation.City,
            DistanceInKm = route.DistanceInKm,
            EstimatedDuration = route.EstimatedDuration,
            IsActive = route.IsActive,
            CreatedAt = route.CreatedAt
        }).ToList();
    }
}

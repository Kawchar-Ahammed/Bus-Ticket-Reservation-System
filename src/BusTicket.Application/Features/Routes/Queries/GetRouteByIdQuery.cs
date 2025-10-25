using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.DTOs;
using MediatR;

namespace BusTicket.Application.Features.Routes.Queries;

public record GetRouteByIdQuery(Guid Id) : IRequest<RouteDetailDto?>;

public class GetRouteByIdQueryHandler : IRequestHandler<GetRouteByIdQuery, RouteDetailDto?>
{
    private readonly IRouteRepository _routeRepository;

    public GetRouteByIdQueryHandler(IRouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public async Task<RouteDetailDto?> Handle(GetRouteByIdQuery request, CancellationToken cancellationToken)
    {
        var route = await _routeRepository.GetByIdAsync(request.Id);

        if (route == null)
            return null;

        return new RouteDetailDto
        {
            Id = route.Id,
            FromCity = route.FromLocation.City,
            ToCity = route.ToLocation.City,
            DistanceInKm = route.DistanceInKm,
            EstimatedDuration = route.EstimatedDuration,
            IsActive = route.IsActive,
            CreatedAt = route.CreatedAt,
            UpdatedAt = route.UpdatedAt,
            CreatedBy = route.CreatedBy,
            UpdatedBy = route.UpdatedBy
        };
    }
}

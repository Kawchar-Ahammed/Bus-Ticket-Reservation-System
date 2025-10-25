using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.DTOs;
using MediatR;

namespace BusTicket.Application.Features.Buses.Queries;

public record GetAllBusesQuery : IRequest<List<BusDto>>;

public class GetAllBusesQueryHandler : IRequestHandler<GetAllBusesQuery, List<BusDto>>
{
    private readonly IBusRepository _busRepository;

    public GetAllBusesQueryHandler(IBusRepository busRepository)
    {
        _busRepository = busRepository;
    }

    public async Task<List<BusDto>> Handle(GetAllBusesQuery request, CancellationToken cancellationToken)
    {
        var buses = await _busRepository.GetAllWithCompanyAsync();

        return buses.Select(bus => new BusDto
        {
            Id = bus.Id,
            BusNumber = bus.BusNumber,
            BusName = bus.BusName,
            CompanyId = bus.CompanyId,
            CompanyName = bus.Company.Name,
            TotalSeats = bus.TotalSeats,
            Description = bus.Description,
            IsActive = bus.IsActive,
            CreatedAt = bus.CreatedAt
        }).ToList();
    }
}

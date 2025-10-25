using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.DTOs;
using MediatR;

namespace BusTicket.Application.Features.Buses.Queries;

public record GetBusByIdQuery(Guid Id) : IRequest<BusDetailDto?>;

public class GetBusByIdQueryHandler : IRequestHandler<GetBusByIdQuery, BusDetailDto?>
{
    private readonly IBusRepository _busRepository;

    public GetBusByIdQueryHandler(IBusRepository busRepository)
    {
        _busRepository = busRepository;
    }

    public async Task<BusDetailDto?> Handle(GetBusByIdQuery request, CancellationToken cancellationToken)
    {
        var bus = await _busRepository.GetByIdWithCompanyAsync(request.Id);

        if (bus == null)
            return null;

        return new BusDetailDto
        {
            Id = bus.Id,
            BusNumber = bus.BusNumber,
            BusName = bus.BusName,
            CompanyId = bus.CompanyId,
            CompanyName = bus.Company.Name,
            TotalSeats = bus.TotalSeats,
            Description = bus.Description,
            IsActive = bus.IsActive,
            CreatedAt = bus.CreatedAt,
            UpdatedAt = bus.UpdatedAt,
            CreatedBy = bus.CreatedBy,
            UpdatedBy = bus.UpdatedBy
        };
    }
}

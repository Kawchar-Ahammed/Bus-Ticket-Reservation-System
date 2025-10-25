using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Exceptions;
using MediatR;

namespace BusTicket.Application.Features.Routes.Commands;

public record DeleteRouteCommand(Guid Id) : IRequest<Unit>;

public class DeleteRouteCommandHandler : IRequestHandler<DeleteRouteCommand, Unit>
{
    private readonly IRouteRepository _routeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRouteCommandHandler(IRouteRepository routeRepository, IUnitOfWork unitOfWork)
    {
        _routeRepository = routeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteRouteCommand request, CancellationToken cancellationToken)
    {
        var route = await _routeRepository.GetByIdAsync(request.Id);
        if (route == null)
        {
            throw new NotFoundException($"Route with ID {request.Id} not found");
        }

        await _routeRepository.DeleteAsync(route);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

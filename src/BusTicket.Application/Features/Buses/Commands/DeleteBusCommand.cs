using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Exceptions;
using MediatR;

namespace BusTicket.Application.Features.Buses.Commands;

public record DeleteBusCommand(Guid Id) : IRequest<Unit>;

public class DeleteBusCommandHandler : IRequestHandler<DeleteBusCommand, Unit>
{
    private readonly IBusRepository _busRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBusCommandHandler(IBusRepository busRepository, IUnitOfWork unitOfWork)
    {
        _busRepository = busRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteBusCommand request, CancellationToken cancellationToken)
    {
        var bus = await _busRepository.GetByIdAsync(request.Id);
        if (bus == null)
        {
            throw new NotFoundException($"Bus with ID {request.Id} not found");
        }

        await _busRepository.DeleteAsync(bus);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

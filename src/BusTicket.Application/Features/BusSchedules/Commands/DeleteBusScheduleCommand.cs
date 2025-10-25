using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Exceptions;
using MediatR;

namespace BusTicket.Application.Features.BusSchedules.Commands;

public record DeleteBusScheduleCommand(Guid Id) : IRequest<Unit>;

public class DeleteBusScheduleCommandHandler : IRequestHandler<DeleteBusScheduleCommand, Unit>
{
    private readonly IBusScheduleRepository _busScheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBusScheduleCommandHandler(IBusScheduleRepository busScheduleRepository, IUnitOfWork unitOfWork)
    {
        _busScheduleRepository = busScheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteBusScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _busScheduleRepository.GetByIdAsync(request.Id);
        if (schedule == null)
        {
            throw new NotFoundException($"Bus schedule with ID {request.Id} not found");
        }

        await _busScheduleRepository.DeleteAsync(schedule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

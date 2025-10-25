using BusTicket.Application.Commands;
using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Services;
using BusTicket.Application.Queries;
using MediatR;

namespace BusTicket.Application.Services;

/// <summary>
/// Application Service for booking operations
/// Acts as a facade over MediatR commands/queries
/// </summary>
public class BookingService : IBookingService
{
    private readonly IMediator _mediator;

    public BookingService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<SeatPlanDto> GetSeatPlanAsync(
        Guid busScheduleId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSeatPlanQuery(busScheduleId);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<BookSeatResultDto> BookSeatAsync(
        BookSeatInputDto input,
        CancellationToken cancellationToken = default)
    {
        var command = new BookSeatCommand(
            input.BusScheduleId,
            input.SeatId,
            input.PassengerName,
            input.PhoneNumber,
            input.Email,
            input.BoardingPoint,
            input.DroppingPoint,
            input.Gender,
            input.Age);

        return await _mediator.Send(command, cancellationToken);
    }
}

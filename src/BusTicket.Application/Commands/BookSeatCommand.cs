using BusTicket.Application.Contracts.DTOs;
using MediatR;

namespace BusTicket.Application.Commands;

/// <summary>
/// Command to book a seat
/// </summary>
public record BookSeatCommand(
    Guid BusScheduleId,
    Guid SeatId,
    string PassengerName,
    string PhoneNumber,
    string? Email,
    string BoardingPoint,
    string DroppingPoint,
    string? Gender,
    int? Age
) : IRequest<BookSeatResultDto>;

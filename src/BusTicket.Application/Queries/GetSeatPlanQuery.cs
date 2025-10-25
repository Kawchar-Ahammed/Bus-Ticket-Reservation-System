using BusTicket.Application.Contracts.DTOs;
using MediatR;

namespace BusTicket.Application.Queries;

/// <summary>
/// Query to get seat plan for a specific bus schedule
/// </summary>
public record GetSeatPlanQuery(
    Guid BusScheduleId
) : IRequest<SeatPlanDto>;

using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Queries;
using MediatR;

namespace BusTicket.Application.Handlers.Queries;

/// <summary>
/// Handler for GetBookingByTicketNumberForAdminQuery
/// </summary>
public class GetBookingByTicketNumberForAdminQueryHandler : IRequestHandler<GetBookingByTicketNumberForAdminQuery, AdminBookingDetailDto?>
{
    private readonly ITicketRepository _ticketRepository;

    public GetBookingByTicketNumberForAdminQueryHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<AdminBookingDetailDto?> Handle(GetBookingByTicketNumberForAdminQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByTicketNumberWithDetailsAsync(request.TicketNumber, cancellationToken);

        if (ticket == null)
            return null;

        return new AdminBookingDetailDto
        {
            TicketId = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            PassengerId = ticket.PassengerId,
            PassengerName = ticket.Passenger.Name,
            PhoneNumber = ticket.Passenger.PhoneNumber.Value,
            Email = ticket.Passenger.Email,
            Gender = ticket.Passenger.Gender?.ToString(),
            Age = ticket.Passenger.Age,
            CompanyId = ticket.BusSchedule.Bus.CompanyId,
            CompanyName = ticket.BusSchedule.Bus.Company.Name,
            BusName = ticket.BusSchedule.Bus.BusName,
            BusNumber = ticket.BusSchedule.Bus.BusNumber,
            FromCity = ticket.BusSchedule.Route.FromLocation.City,
            ToCity = ticket.BusSchedule.Route.ToLocation.City,
            JourneyDate = ticket.BusSchedule.JourneyDate,
            DepartureTime = ticket.BusSchedule.DepartureTime,
            ArrivalTime = ticket.BusSchedule.ArrivalTime,
            SeatNumber = ticket.Seat.SeatNumber,
            BoardingPoint = ticket.BoardingPoint.City,
            DroppingPoint = ticket.DroppingPoint.City,
            BookingDate = ticket.BookingDate,
            Fare = ticket.Fare.Amount,
            Currency = ticket.Fare.Currency,
            BookingStatus = GetBookingStatus(ticket.IsConfirmed, ticket.IsCancelled),
            IsConfirmed = ticket.IsConfirmed,
            IsCancelled = ticket.IsCancelled,
            ConfirmationDate = ticket.ConfirmationDate,
            BusScheduleId = ticket.BusScheduleId,
            SeatId = ticket.SeatId,
            CreatedAt = ticket.CreatedAt,
            CancelledAt = ticket.CancellationDate,
            CancellationReason = ticket.CancellationReason
        };
    }

    private static string GetBookingStatus(bool isConfirmed, bool isCancelled)
    {
        if (isCancelled) return "Cancelled";
        if (isConfirmed) return "Confirmed";
        return "Pending";
    }
}

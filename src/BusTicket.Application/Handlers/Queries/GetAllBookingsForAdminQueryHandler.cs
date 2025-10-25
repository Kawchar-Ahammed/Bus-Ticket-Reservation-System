using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Queries;
using MediatR;

namespace BusTicket.Application.Handlers.Queries;

/// <summary>
/// Handler for GetAllBookingsForAdminQuery
/// </summary>
public class GetAllBookingsForAdminQueryHandler : IRequestHandler<GetAllBookingsForAdminQuery, List<AdminBookingDto>>
{
    private readonly ITicketRepository _ticketRepository;

    public GetAllBookingsForAdminQueryHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<List<AdminBookingDto>> Handle(GetAllBookingsForAdminQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetAllTicketsForAdminAsync(
            request.CompanyId,
            request.BookingStatus,
            null, // paymentStatus not used in Ticket entity
            request.JourneyDateFrom,
            request.JourneyDateTo,
            request.BookingDateFrom,
            request.BookingDateTo,
            request.SearchTerm,
            cancellationToken);

        return tickets.Select(t => new AdminBookingDto
        {
            TicketId = t.Id,
            TicketNumber = t.TicketNumber,
            PassengerId = t.PassengerId,
            PassengerName = t.Passenger.Name,
            PhoneNumber = t.Passenger.PhoneNumber.Value,
            Email = t.Passenger.Email,
            CompanyId = t.BusSchedule.Bus.CompanyId,
            CompanyName = t.BusSchedule.Bus.Company.Name,
            BusName = t.BusSchedule.Bus.BusName,
            BusNumber = t.BusSchedule.Bus.BusNumber,
            FromCity = t.BusSchedule.Route.FromLocation.City,
            ToCity = t.BusSchedule.Route.ToLocation.City,
            JourneyDate = t.BusSchedule.JourneyDate,
            DepartureTime = t.BusSchedule.DepartureTime,
            SeatNumber = t.Seat.SeatNumber,
            BookingDate = t.BookingDate,
            Fare = t.Fare.Amount,
            Currency = t.Fare.Currency,
            BookingStatus = GetBookingStatus(t.IsConfirmed, t.IsCancelled),
            IsConfirmed = t.IsConfirmed,
            IsCancelled = t.IsCancelled,
            ConfirmationDate = t.ConfirmationDate
        }).ToList();
    }

    private static string GetBookingStatus(bool isConfirmed, bool isCancelled)
    {
        if (isCancelled) return "Cancelled";
        if (isConfirmed) return "Confirmed";
        return "Pending";
    }
}

using BusTicket.Application.Commands;
using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Contracts.Services;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using BusTicket.Domain.Exceptions;
using BusTicket.Domain.Services;
using BusTicket.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicket.Application.Handlers.Commands;

/// <summary>
/// Handler for BookSeatCommand
/// Implements the booking use case with transaction safety
/// </summary>
public class BookSeatCommandHandler : IRequestHandler<BookSeatCommand, BookSeatResultDto>
{
    private readonly IBusScheduleRepository _busScheduleRepository;
    private readonly ISeatRepository _seatRepository;
    private readonly IPassengerRepository _passengerRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ISeatBookingDomainService _bookingDomainService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<BookSeatCommandHandler> _logger;

    public BookSeatCommandHandler(
        IBusScheduleRepository busScheduleRepository,
        ISeatRepository seatRepository,
        IPassengerRepository passengerRepository,
        ITicketRepository ticketRepository,
        ISeatBookingDomainService bookingDomainService,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ISmsService smsService,
        ILogger<BookSeatCommandHandler> logger)
    {
        _busScheduleRepository = busScheduleRepository;
        _seatRepository = seatRepository;
        _passengerRepository = passengerRepository;
        _ticketRepository = ticketRepository;
        _bookingDomainService = bookingDomainService;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task<BookSeatResultDto> Handle(
        BookSeatCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing seat booking: BusSchedule={BusScheduleId}, Seat={SeatId}, Passenger={PassengerName}",
            request.BusScheduleId,
            request.SeatId,
            request.PassengerName);

        try
        {
            // Begin transaction to ensure atomicity
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // 1. Get and validate bus schedule
            var busSchedule = await _busScheduleRepository.GetWithDetailsAsync(
                request.BusScheduleId,
                cancellationToken);

            if (busSchedule == null)
            {
                throw new EntityNotFoundException(nameof(BusSchedule), request.BusScheduleId);
            }

            if (!busSchedule.IsActive)
            {
                throw new BusinessRuleViolationException("This bus schedule is no longer active");
            }

            // 2. Get and validate seat
            var seat = await _seatRepository.GetSeatWithScheduleAsync(
                request.SeatId,
                cancellationToken);

            if (seat == null)
            {
                throw new EntityNotFoundException(nameof(Seat), request.SeatId);
            }

            if (seat.BusScheduleId != request.BusScheduleId)
            {
                throw new BusinessRuleViolationException("Seat does not belong to the selected bus schedule");
            }

            if (!_bookingDomainService.CanBookSeat(seat))
            {
                throw new SeatNotAvailableException(seat.SeatNumber);
            }

            // 3. Get or create passenger
            var phoneNumber = PhoneNumber.Create(request.PhoneNumber);
            var passenger = await _passengerRepository.FindByPhoneNumberAsync(
                phoneNumber.Value,
                cancellationToken);

            if (passenger == null)
            {
                Gender? gender = null;
                if (!string.IsNullOrWhiteSpace(request.Gender))
                {
                    gender = Enum.Parse<Gender>(request.Gender);
                }

                passenger = Passenger.Create(
                    request.PassengerName,
                    phoneNumber,
                    request.Email,
                    gender,
                    request.Age);

                passenger = await _passengerRepository.AddAsync(passenger, cancellationToken);
                _logger.LogInformation("Created new passenger: {PassengerId}", passenger.Id);
            }
            else
            {
                _logger.LogInformation("Using existing passenger: {PassengerId}", passenger.Id);
            }

            // 4. Book the seat using domain service
            var ticket = await _bookingDomainService.BookSeatAsync(
                seat,
                passenger,
                busSchedule,
                cancellationToken);

            // 5. Persist changes
            await _ticketRepository.AddAsync(ticket, cancellationToken);
            await _seatRepository.UpdateAsync(seat, cancellationToken);

            // 6. Save changes and commit transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully booked seat: Ticket={TicketNumber}, Seat={SeatNumber}",
                ticket.TicketNumber,
                seat.SeatNumber);

            // 7. Send booking confirmation notifications (fire and forget - don't fail booking if notifications fail)
            _ = Task.Run(async () =>
            {
                try
                {
                    var notificationDto = new BookingNotificationDto
                    {
                        TicketNumber = ticket.TicketNumber,
                        PassengerName = passenger.Name,
                        Email = passenger.Email ?? string.Empty,
                        PhoneNumber = passenger.PhoneNumber.Value,
                        CompanyName = busSchedule.Bus?.Company?.Name ?? "Bus Company",
                        BusName = busSchedule.Bus?.BusName ?? "N/A",
                        FromCity = busSchedule.Route?.FromLocation?.City ?? "Origin",
                        ToCity = busSchedule.Route?.ToLocation?.City ?? "Destination",
                        JourneyDate = busSchedule.JourneyDate,
                        DepartureTime = busSchedule.DepartureTime.ToString(@"hh\:mm"),
                        SeatNumber = seat.SeatNumber,
                        BoardingPoint = busSchedule.BoardingPoint?.ToString() ?? "N/A",
                        DroppingPoint = busSchedule.DroppingPoint?.ToString() ?? "N/A",
                        Fare = ticket.Fare.Amount,
                        Currency = ticket.Fare.Currency
                    };

                    // Send email confirmation
                    if (!string.IsNullOrEmpty(passenger.Email))
                    {
                        await _emailService.SendBookingConfirmationAsync(notificationDto, CancellationToken.None);
                    }

                    // Send SMS confirmation
                    await _smsService.SendBookingConfirmationSmsAsync(notificationDto, CancellationToken.None);

                    _logger.LogInformation("Booking confirmation notifications sent for ticket {TicketNumber}", ticket.TicketNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send booking confirmation notifications for ticket {TicketNumber}", ticket.TicketNumber);
                }
            }, CancellationToken.None);

            // Return success result
            return new BookSeatResultDto
            {
                Success = true,
                Message = "Seat booked successfully",
                TicketId = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                PassengerId = passenger.Id,
                PassengerName = passenger.Name,
                SeatNumber = seat.SeatNumber,
                Fare = ticket.Fare.Amount,
                Currency = ticket.Fare.Currency,
                BookingDate = ticket.BookingDate
            };
        }
        catch (Exception ex)
        {
            // Rollback transaction on any error
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            _logger.LogError(
                ex,
                "Failed to book seat: BusSchedule={BusScheduleId}, Seat={SeatId}",
                request.BusScheduleId,
                request.SeatId);

            // Return failure result with error message
            return new BookSeatResultDto
            {
                Success = false,
                Message = ex is DomainException
                    ? ex.Message
                    : "An error occurred while booking the seat. Please try again."
            };
        }
    }
}

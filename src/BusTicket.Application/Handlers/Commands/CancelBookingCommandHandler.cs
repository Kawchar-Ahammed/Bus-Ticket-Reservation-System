using BusTicket.Application.Commands;
using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Contracts.Services;
using BusTicket.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicket.Application.Handlers.Commands;

/// <summary>
/// Handler for CancelBookingCommand
/// Implements cancellation policy:
/// - Can cancel up to 2 hours before departure
/// - Full refund if cancelled 24+ hours before
/// - 50% refund if cancelled 2-24 hours before
/// - No refund if cancelled less than 2 hours before
/// </summary>
public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, CancelBookingResultDto>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ISeatRepository _seatRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<CancelBookingCommandHandler> _logger;

    public CancelBookingCommandHandler(
        ITicketRepository ticketRepository,
        ISeatRepository seatRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ISmsService smsService,
        ILogger<CancelBookingCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _seatRepository = seatRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task<CancelBookingResultDto> Handle(
        CancelBookingCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing cancellation for ticket: {TicketNumber}, Phone: {PhoneNumber}",
            request.TicketNumber,
            request.PhoneNumber);

        try
        {
            // Get ticket with details
            var ticket = await _ticketRepository.GetByTicketNumberWithDetailsAsync(
                request.TicketNumber,
                cancellationToken);

            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found: {TicketNumber}", request.TicketNumber);
                return new CancelBookingResultDto
                {
                    Success = false,
                    Message = "Ticket not found"
                };
            }

            // Verify phone number matches
            if (ticket.Passenger.PhoneNumber.Value != request.PhoneNumber)
            {
                _logger.LogWarning(
                    "Phone number mismatch for ticket: {TicketNumber}",
                    request.TicketNumber);
                return new CancelBookingResultDto
                {
                    Success = false,
                    Message = "Phone number does not match the booking"
                };
            }

            // Check if already cancelled
            if (ticket.IsCancelled)
            {
                _logger.LogWarning("Ticket already cancelled: {TicketNumber}", request.TicketNumber);
                return new CancelBookingResultDto
                {
                    Success = false,
                    Message = "This ticket has already been cancelled",
                    TicketNumber = ticket.TicketNumber,
                    CancellationDate = ticket.CancellationDate
                };
            }

            // Check if journey has already started or completed
            var journeyDateTime = ticket.BusSchedule.JourneyDate.Date
                .Add(ticket.BusSchedule.DepartureTime);

            var now = DateTime.UtcNow;
            var hoursUntilDeparture = (journeyDateTime - now).TotalHours;

            if (hoursUntilDeparture < 0)
            {
                _logger.LogWarning(
                    "Cannot cancel ticket after departure: {TicketNumber}",
                    request.TicketNumber);
                return new CancelBookingResultDto
                {
                    Success = false,
                    Message = "Cannot cancel ticket after journey has started"
                };
            }

            if (hoursUntilDeparture < 2)
            {
                _logger.LogWarning(
                    "Cannot cancel ticket less than 2 hours before departure: {TicketNumber}",
                    request.TicketNumber);
                return new CancelBookingResultDto
                {
                    Success = false,
                    Message = "Cannot cancel ticket less than 2 hours before departure"
                };
            }

            // Calculate refund amount based on cancellation policy
            decimal refundPercentage = hoursUntilDeparture >= 24 ? 100m : 50m;
            decimal refundAmount = ticket.Fare.Amount * (refundPercentage / 100m);

            // Begin transaction
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Cancel the ticket
            ticket.Cancel(request.CancellationReason);

            // Update ticket
            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            // Get and update seat status back to Available
            var seat = await _seatRepository.GetByIdAsync(ticket.SeatId, cancellationToken);
            if (seat != null)
            {
                seat.CancelBooking();
                await _seatRepository.UpdateAsync(seat, cancellationToken);
                _logger.LogInformation(
                    "Seat {SeatNumber} status updated to Available",
                    seat.SeatNumber);
            }

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Ticket cancelled successfully: {TicketNumber}, Refund: {RefundAmount} ({RefundPercentage}%)",
                request.TicketNumber,
                refundAmount,
                refundPercentage);

            // Send cancellation confirmation notifications (fire and forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    var cancellationDto = new CancellationNotificationDto
                    {
                        TicketNumber = ticket.TicketNumber,
                        PassengerName = ticket.Passenger.Name,
                        Email = ticket.Passenger.Email ?? string.Empty,
                        PhoneNumber = ticket.Passenger.PhoneNumber.Value,
                        CompanyName = ticket.BusSchedule.Bus?.Company?.Name ?? "Bus Company",
                        FromCity = ticket.BusSchedule.Route?.FromLocation.City ?? "N/A",
                        ToCity = ticket.BusSchedule.Route?.ToLocation.City ?? "N/A",
                        JourneyDate = ticket.BusSchedule.JourneyDate,
                        RefundAmount = refundAmount,
                        Currency = ticket.Fare.Currency,
                        CancellationReason = request.CancellationReason ?? "Not specified"
                    };

                    // Send email notification if email exists
                    if (!string.IsNullOrEmpty(ticket.Passenger.Email))
                    {
                        await _emailService.SendCancellationConfirmationAsync(
                            cancellationDto,
                            CancellationToken.None);
                        _logger.LogInformation(
                            "Cancellation confirmation email sent to {Email} for ticket {TicketNumber}",
                            ticket.Passenger.Email,
                            ticket.TicketNumber);
                    }

                    // Send SMS notification
                    await _smsService.SendCancellationConfirmationSmsAsync(
                        cancellationDto,
                        CancellationToken.None);
                    _logger.LogInformation(
                        "Cancellation confirmation SMS sent to {PhoneNumber} for ticket {TicketNumber}",
                        ticket.Passenger.PhoneNumber.Value,
                        ticket.TicketNumber);
                }
                catch (Exception ex)
                {
                    // Log notification errors but don't fail the cancellation
                    _logger.LogError(
                        ex,
                        "Failed to send cancellation confirmation notifications for ticket {TicketNumber}",
                        ticket.TicketNumber);
                }
            }, CancellationToken.None);

            return new CancelBookingResultDto
            {
                Success = true,
                Message = $"Booking cancelled successfully. Refund amount: {ticket.Fare.Currency} {refundAmount:F2}",
                TicketNumber = ticket.TicketNumber,
                RefundAmount = refundAmount,
                RefundStatus = hoursUntilDeparture >= 24 ? "Full Refund (100%)" : "Partial Refund (50%)",
                CancellationDate = ticket.CancellationDate
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error cancelling booking: {TicketNumber}", request.TicketNumber);

            return new CancelBookingResultDto
            {
                Success = false,
                Message = "An error occurred while cancelling the booking. Please try again."
            };
        }
    }
}

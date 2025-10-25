using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace BusTicket.Infrastructure.Services;

/// <summary>
/// Background service for sending journey reminder notifications
/// Sends reminders at 24 hours and 1 hour before journey departure
/// </summary>
public class JourneyReminderService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<JourneyReminderService> _logger;

    public JourneyReminderService(
        ITicketRepository ticketRepository,
        IEmailService emailService,
        ISmsService smsService,
        ILogger<JourneyReminderService> logger)
    {
        _ticketRepository = ticketRepository;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    /// <summary>
    /// Checks for upcoming journeys and sends 24-hour reminders
    /// </summary>
    public async Task SendTwentyFourHourRemindersAsync()
    {
        _logger.LogInformation("Starting 24-hour journey reminder job at {Time}", DateTime.UtcNow);

        try
        {
            // Calculate the time window for 24-hour reminders
            // We want journeys that are departing approximately 24 hours from now
            // Using a 30-minute window (23:45 to 24:15) to avoid missing due to timing
            var targetTime = DateTime.UtcNow.AddHours(24);
            var windowStart = targetTime.AddMinutes(-15);
            var windowEnd = targetTime.AddMinutes(15);

            // Get all confirmed (not cancelled) tickets for journeys in this time window
            var upcomingTickets = await _ticketRepository.GetUpcomingTicketsWithDetailsAsync(
                windowStart,
                windowEnd,
                CancellationToken.None);

            _logger.LogInformation(
                "Found {Count} tickets for 24-hour reminders (window: {Start} to {End})",
                upcomingTickets.Count(),
                windowStart,
                windowEnd);

            foreach (var ticket in upcomingTickets)
            {
                try
                {
                    var reminderDto = new BookingNotificationDto
                    {
                        TicketNumber = ticket.TicketNumber,
                        PassengerName = ticket.Passenger.Name,
                        Email = ticket.Passenger.Email ?? string.Empty,
                        PhoneNumber = ticket.Passenger.PhoneNumber.Value,
                        CompanyName = ticket.BusSchedule.Bus?.Company?.Name ?? "Bus Company",
                        BusName = ticket.BusSchedule.Bus?.BusName ?? "N/A",
                        FromCity = ticket.BusSchedule.Route?.FromLocation.City ?? "N/A",
                        ToCity = ticket.BusSchedule.Route?.ToLocation.City ?? "N/A",
                        JourneyDate = ticket.BusSchedule.JourneyDate,
                        DepartureTime = ticket.BusSchedule.DepartureTime.ToString(@"hh\:mm"),
                        SeatNumber = ticket.Seat?.SeatNumber ?? "N/A",
                        BoardingPoint = ticket.BoardingPoint.City,
                        DroppingPoint = ticket.DroppingPoint.City,
                        Fare = ticket.Fare.Amount,
                        Currency = ticket.Fare.Currency
                    };

                    // Send email reminder if email exists
                    if (!string.IsNullOrEmpty(ticket.Passenger.Email))
                    {
                        await _emailService.SendJourneyReminderAsync(
                            reminderDto,
                            24,
                            CancellationToken.None);
                        
                        _logger.LogInformation(
                            "24-hour reminder email sent to {Email} for ticket {TicketNumber}",
                            ticket.Passenger.Email,
                            ticket.TicketNumber);
                    }

                    // Send SMS reminder
                    await _smsService.SendJourneyReminderSmsAsync(
                        reminderDto,
                        24,
                        CancellationToken.None);
                    
                    _logger.LogInformation(
                        "24-hour reminder SMS sent to {PhoneNumber} for ticket {TicketNumber}",
                        ticket.Passenger.PhoneNumber.Value,
                        ticket.TicketNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send 24-hour reminder for ticket {TicketNumber}",
                        ticket.TicketNumber);
                }
            }

            _logger.LogInformation(
                "Completed 24-hour journey reminder job. Sent {Count} reminders",
                upcomingTickets.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in 24-hour journey reminder job");
        }
    }

    /// <summary>
    /// Checks for upcoming journeys and sends 1-hour reminders
    /// </summary>
    public async Task SendOneHourRemindersAsync()
    {
        _logger.LogInformation("Starting 1-hour journey reminder job at {Time}", DateTime.UtcNow);

        try
        {
            // Calculate the time window for 1-hour reminders
            // We want journeys that are departing approximately 1 hour from now
            // Using a 15-minute window (0:52 to 1:08) to avoid missing due to timing
            var targetTime = DateTime.UtcNow.AddHours(1);
            var windowStart = targetTime.AddMinutes(-8);
            var windowEnd = targetTime.AddMinutes(8);

            // Get all confirmed (not cancelled) tickets for journeys in this time window
            var upcomingTickets = await _ticketRepository.GetUpcomingTicketsWithDetailsAsync(
                windowStart,
                windowEnd,
                CancellationToken.None);

            _logger.LogInformation(
                "Found {Count} tickets for 1-hour reminders (window: {Start} to {End})",
                upcomingTickets.Count(),
                windowStart,
                windowEnd);

            foreach (var ticket in upcomingTickets)
            {
                try
                {
                    var reminderDto = new BookingNotificationDto
                    {
                        TicketNumber = ticket.TicketNumber,
                        PassengerName = ticket.Passenger.Name,
                        Email = ticket.Passenger.Email ?? string.Empty,
                        PhoneNumber = ticket.Passenger.PhoneNumber.Value,
                        CompanyName = ticket.BusSchedule.Bus?.Company?.Name ?? "Bus Company",
                        BusName = ticket.BusSchedule.Bus?.BusName ?? "N/A",
                        FromCity = ticket.BusSchedule.Route?.FromLocation.City ?? "N/A",
                        ToCity = ticket.BusSchedule.Route?.ToLocation.City ?? "N/A",
                        JourneyDate = ticket.BusSchedule.JourneyDate,
                        DepartureTime = ticket.BusSchedule.DepartureTime.ToString(@"hh\:mm"),
                        SeatNumber = ticket.Seat?.SeatNumber ?? "N/A",
                        BoardingPoint = ticket.BoardingPoint.City,
                        DroppingPoint = ticket.DroppingPoint.City,
                        Fare = ticket.Fare.Amount,
                        Currency = ticket.Fare.Currency
                    };

                    // Send email reminder if email exists
                    if (!string.IsNullOrEmpty(ticket.Passenger.Email))
                    {
                        await _emailService.SendJourneyReminderAsync(
                            reminderDto,
                            1,
                            CancellationToken.None);
                        
                        _logger.LogInformation(
                            "1-hour reminder email sent to {Email} for ticket {TicketNumber}",
                            ticket.Passenger.Email,
                            ticket.TicketNumber);
                    }

                    // Send SMS reminder
                    await _smsService.SendJourneyReminderSmsAsync(
                        reminderDto,
                        1,
                        CancellationToken.None);
                    
                    _logger.LogInformation(
                        "1-hour reminder SMS sent to {PhoneNumber} for ticket {TicketNumber}",
                        ticket.Passenger.PhoneNumber.Value,
                        ticket.TicketNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send 1-hour reminder for ticket {TicketNumber}",
                        ticket.TicketNumber);
                }
            }

            _logger.LogInformation(
                "Completed 1-hour journey reminder job. Sent {Count} reminders",
                upcomingTickets.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in 1-hour journey reminder job");
        }
    }
}

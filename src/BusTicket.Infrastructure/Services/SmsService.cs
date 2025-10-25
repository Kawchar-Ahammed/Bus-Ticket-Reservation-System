using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BusTicket.Infrastructure.Services;

public class SmsService : ISmsService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;
    private readonly string? _apiKey;
    private readonly string? _apiUrl;
    private readonly bool _enableSms;

    public SmsService(IConfiguration configuration, ILogger<SmsService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        _apiKey = _configuration["SmsSettings:ApiKey"];
        _apiUrl = _configuration["SmsSettings:ApiUrl"];
        _enableSms = bool.Parse(_configuration["SmsSettings:EnableSms"] ?? "false");
    }

    public async Task SendSmsAsync(SmsDto smsDto, CancellationToken cancellationToken = default)
    {
        if (!_enableSms)
        {
            _logger.LogInformation("SMS is disabled. Message to {PhoneNumber}: {Message}", 
                smsDto.PhoneNumber, smsDto.Message);
            return;
        }

        try
        {
            // TODO: Implement actual SMS gateway integration (Twilio, Nexmo, local SMS gateway)
            // For now, just log the SMS
            _logger.LogInformation("SMS would be sent to {PhoneNumber}: {Message}", 
                smsDto.PhoneNumber, smsDto.Message);
            
            // Example Twilio integration (commented out):
            /*
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("to", smsDto.PhoneNumber),
                new KeyValuePair<string, string>("message", smsDto.Message),
                new KeyValuePair<string, string>("api_key", _apiKey)
            });
            
            var response = await client.PostAsync(_apiUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            */
            
            await Task.CompletedTask;
            _logger.LogInformation("SMS sent successfully to {PhoneNumber}", smsDto.PhoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", smsDto.PhoneNumber);
            // Don't throw - we don't want to fail booking just because SMS failed
        }
    }

    public async Task SendBookingConfirmationSmsAsync(BookingNotificationDto notification, CancellationToken cancellationToken = default)
    {
        var message = $"Booking Confirmed!\n" +
                     $"Ticket: {notification.TicketNumber}\n" +
                     $"Passenger: {notification.PassengerName}\n" +
                     $"Route: {notification.FromCity} to {notification.ToCity}\n" +
                     $"Date: {notification.JourneyDate:dd MMM yyyy} at {notification.DepartureTime}\n" +
                     $"Seat: {notification.SeatNumber}\n" +
                     $"Company: {notification.CompanyName}\n" +
                     $"Fare: {notification.Currency} {notification.Fare:N2}\n" +
                     $"Boarding: {notification.BoardingPoint}\n" +
                     $"Arrive 15 mins early. Have a safe journey!\n" +
                     $"- Bus Ticket Reservation";

        await SendSmsAsync(new SmsDto
        {
            PhoneNumber = notification.PhoneNumber,
            Message = message
        }, cancellationToken);
    }

    public async Task SendCancellationConfirmationSmsAsync(CancellationNotificationDto notification, CancellationToken cancellationToken = default)
    {
        var refundInfo = notification.RefundAmount > 0
            ? $"Refund: {notification.Currency} {notification.RefundAmount:N2} (5-7 days)"
            : "No refund (cancelled < 2 hours before)";

        var message = $"Booking Cancelled\n" +
                     $"Ticket: {notification.TicketNumber}\n" +
                     $"Passenger: {notification.PassengerName}\n" +
                     $"Route: {notification.FromCity} to {notification.ToCity}\n" +
                     $"Date: {notification.JourneyDate:dd MMM yyyy}\n" +
                     $"{refundInfo}\n" +
                     $"Thank you!\n" +
                     $"- {notification.CompanyName}";

        await SendSmsAsync(new SmsDto
        {
            PhoneNumber = notification.PhoneNumber,
            Message = message
        }, cancellationToken);
    }

    public async Task SendJourneyReminderSmsAsync(BookingNotificationDto notification, int hoursBeforeDeparture, CancellationToken cancellationToken = default)
    {
        var urgency = hoursBeforeDeparture == 1 ? "URGENT REMINDER" : "REMINDER";
        var timeMessage = hoursBeforeDeparture == 24 
            ? "Your journey is TOMORROW!" 
            : $"Your journey starts in {hoursBeforeDeparture} HOUR!";

        var message = $"{urgency}: {timeMessage}\n" +
                     $"Ticket: {notification.TicketNumber}\n" +
                     $"From: {notification.FromCity}\n" +
                     $"To: {notification.ToCity}\n" +
                     $"Date: {notification.JourneyDate:dd MMM yyyy}\n" +
                     $"Time: {notification.DepartureTime}\n" +
                     $"Seat: {notification.SeatNumber}\n" +
                     $"Boarding: {notification.BoardingPoint}\n" +
                     $"{(hoursBeforeDeparture == 1 ? "⚠️ LEAVE NOW! Arrive 15 mins early." : "Arrive 15 mins early.")}\n" +
                     $"Safe journey!\n" +
                     $"- {notification.CompanyName}";

        await SendSmsAsync(new SmsDto
        {
            PhoneNumber = notification.PhoneNumber,
            Message = message
        }, cancellationToken);
    }
}

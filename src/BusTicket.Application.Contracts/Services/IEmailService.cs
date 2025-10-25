using BusTicket.Application.Contracts.DTOs;

namespace BusTicket.Application.Contracts.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailDto emailDto, CancellationToken cancellationToken = default);
    Task SendBookingConfirmationAsync(BookingNotificationDto notification, CancellationToken cancellationToken = default);
    Task SendCancellationConfirmationAsync(CancellationNotificationDto notification, CancellationToken cancellationToken = default);
    Task SendJourneyReminderAsync(BookingNotificationDto notification, int hoursBeforeDeparture, CancellationToken cancellationToken = default);
}

public interface ISmsService
{
    Task SendSmsAsync(SmsDto smsDto, CancellationToken cancellationToken = default);
    Task SendBookingConfirmationSmsAsync(BookingNotificationDto notification, CancellationToken cancellationToken = default);
    Task SendCancellationConfirmationSmsAsync(CancellationNotificationDto notification, CancellationToken cancellationToken = default);
    Task SendJourneyReminderSmsAsync(BookingNotificationDto notification, int hoursBeforeDeparture, CancellationToken cancellationToken = default);
}

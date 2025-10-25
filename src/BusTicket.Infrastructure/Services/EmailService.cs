using System.Net;
using System.Net.Mail;
using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BusTicket.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
        _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
        _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@busticket.com";
        _fromName = _configuration["EmailSettings:FromName"] ?? "Bus Ticket Reservation";
        _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
    }

    public async Task SendEmailAsync(EmailDto emailDto, CancellationToken cancellationToken = default)
    {
        try
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = emailDto.Subject,
                Body = emailDto.Body,
                IsBodyHtml = emailDto.IsHtml
            };

            mailMessage.To.Add(emailDto.To);

            if (emailDto.Cc != null)
            {
                foreach (var cc in emailDto.Cc)
                {
                    mailMessage.CC.Add(cc);
                }
            }

            if (emailDto.Bcc != null)
            {
                foreach (var bcc in emailDto.Bcc)
                {
                    mailMessage.Bcc.Add(bcc);
                }
            }

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = _enableSsl
            };

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
            _logger.LogInformation("Email sent successfully to {To}", emailDto.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", emailDto.To);
            // Don't throw - we don't want to fail booking just because email failed
        }
    }

    public async Task SendBookingConfirmationAsync(BookingNotificationDto notification, CancellationToken cancellationToken = default)
    {
        var subject = $"Booking Confirmed - {notification.TicketNumber}";
        var body = GenerateBookingConfirmationHtml(notification);

        await SendEmailAsync(new EmailDto
        {
            To = notification.Email,
            Subject = subject,
            Body = body,
            IsHtml = true
        }, cancellationToken);
    }

    public async Task SendCancellationConfirmationAsync(CancellationNotificationDto notification, CancellationToken cancellationToken = default)
    {
        var subject = $"Booking Cancelled - {notification.TicketNumber}";
        var body = GenerateCancellationConfirmationHtml(notification);

        await SendEmailAsync(new EmailDto
        {
            To = notification.Email,
            Subject = subject,
            Body = body,
            IsHtml = true
        }, cancellationToken);
    }

    public async Task SendJourneyReminderAsync(BookingNotificationDto notification, int hoursBeforeDeparture, CancellationToken cancellationToken = default)
    {
        var subject = hoursBeforeDeparture == 24 
            ? $"Journey Tomorrow - {notification.TicketNumber}"
            : $"Journey in {hoursBeforeDeparture} Hour - {notification.TicketNumber}";
        
        var body = GenerateJourneyReminderHtml(notification, hoursBeforeDeparture);

        await SendEmailAsync(new EmailDto
        {
            To = notification.Email,
            Subject = subject,
            Body = body,
            IsHtml = true
        }, cancellationToken);
    }

    private string GenerateBookingConfirmationHtml(BookingNotificationDto notification)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #1976d2 0%, #1565c0 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .header p {{ margin: 10px 0 0 0; font-size: 16px; opacity: 0.9; }}
        .content {{ background: #fff; padding: 30px; border: 1px solid #e0e0e0; }}
        .success-badge {{ background: #4caf50; color: white; padding: 8px 20px; border-radius: 20px; display: inline-block; margin: 20px 0; font-weight: bold; }}
        .ticket-box {{ background: #f5f5f5; border-left: 4px solid #1976d2; padding: 20px; margin: 20px 0; border-radius: 4px; }}
        .ticket-number {{ font-size: 24px; font-weight: bold; color: #1976d2; font-family: 'Courier New', monospace; }}
        .info-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin: 20px 0; }}
        .info-item {{ }}
        .info-label {{ font-size: 12px; color: #666; text-transform: uppercase; margin-bottom: 5px; }}
        .info-value {{ font-size: 16px; font-weight: bold; color: #333; }}
        .route-section {{ background: white; border: 2px solid #1976d2; padding: 20px; margin: 20px 0; border-radius: 8px; }}
        .route-row {{ display: flex; align-items: center; justify-content: space-between; }}
        .route-point {{ flex: 1; }}
        .route-arrow {{ flex: 0 0 auto; padding: 0 20px; font-size: 24px; color: #1976d2; }}
        .route-city {{ font-size: 20px; font-weight: bold; color: #1976d2; }}
        .route-address {{ font-size: 14px; color: #666; margin-top: 5px; }}
        .important-box {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; border-radius: 4px; }}
        .important-box strong {{ color: #856404; }}
        .footer {{ background: #f5f5f5; padding: 20px; text-align: center; border-radius: 0 0 8px 8px; font-size: 14px; color: #666; }}
        .button {{ display: inline-block; background: #1976d2; color: white; padding: 12px 30px; text-decoration: none; border-radius: 4px; margin: 10px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Booking Confirmed!</h1>
            <p>Your bus ticket has been successfully booked</p>
        </div>
        
        <div class='content'>
            <div class='success-badge'>✓ BOOKING SUCCESSFUL</div>
            
            <div class='ticket-box'>
                <div class='info-label'>Ticket Number</div>
                <div class='ticket-number'>{notification.TicketNumber}</div>
            </div>

            <h2 style='color: #1976d2; border-bottom: 2px solid #1976d2; padding-bottom: 10px;'>Journey Details</h2>
            
            <div class='route-section'>
                <div class='route-row'>
                    <div class='route-point'>
                        <div class='route-city'>📍 {notification.FromCity}</div>
                        <div class='route-address'>{notification.BoardingPoint}</div>
                    </div>
                    <div class='route-arrow'>→</div>
                    <div class='route-point' style='text-align: right;'>
                        <div class='route-city'>{notification.ToCity} 🏁</div>
                        <div class='route-address'>{notification.DroppingPoint}</div>
                    </div>
                </div>
            </div>

            <div class='info-grid'>
                <div class='info-item'>
                    <div class='info-label'>Passenger Name</div>
                    <div class='info-value'>{notification.PassengerName}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Company</div>
                    <div class='info-value'>{notification.CompanyName}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Bus</div>
                    <div class='info-value'>{notification.BusName}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Seat Number</div>
                    <div class='info-value'>{notification.SeatNumber}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Journey Date</div>
                    <div class='info-value'>{notification.JourneyDate:dd MMM yyyy}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Departure Time</div>
                    <div class='info-value'>{notification.DepartureTime}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Phone</div>
                    <div class='info-value'>{notification.PhoneNumber}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Total Fare</div>
                    <div class='info-value' style='color: #4caf50;'>{notification.Currency} {notification.Fare:N2}</div>
                </div>
            </div>

            <div class='important-box'>
                <strong>⚠️ Important Instructions:</strong>
                <ul style='margin: 10px 0; padding-left: 20px;'>
                    <li>Please arrive at the boarding point <strong>15 minutes before departure</strong></li>
                    <li>Carry this ticket number and a valid ID for verification</li>
                    <li>You will receive a reminder 24 hours and 1 hour before your journey</li>
                    <li>For cancellation, visit our website or contact customer support</li>
                </ul>
            </div>

            <div style='text-align: center; margin: 30px 0;'>
                <p style='margin: 0 0 10px 0; color: #666;'>Need to print your ticket?</p>
                <a href='http://localhost:4200/print-ticket/{notification.TicketNumber}' class='button'>
                    🖨️ Print Ticket
                </a>
            </div>
        </div>

        <div class='footer'>
            <p style='margin: 0 0 10px 0;'>Thank you for choosing {notification.CompanyName}!</p>
            <p style='margin: 0; font-size: 12px;'>Have a safe and pleasant journey 🚌</p>
            <p style='margin: 15px 0 0 0; font-size: 12px; color: #999;'>
                © 2025 Bus Ticket Reservation System. All rights reserved.
            </p>
        </div>
    </div>
</body>
</html>";
    }

    private string GenerateCancellationConfirmationHtml(CancellationNotificationDto notification)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #d32f2f 0%, #c62828 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .header p {{ margin: 10px 0 0 0; font-size: 16px; opacity: 0.9; }}
        .content {{ background: #fff; padding: 30px; border: 1px solid #e0e0e0; }}
        .cancelled-badge {{ background: #f44336; color: white; padding: 8px 20px; border-radius: 20px; display: inline-block; margin: 20px 0; font-weight: bold; }}
        .ticket-box {{ background: #f5f5f5; border-left: 4px solid #f44336; padding: 20px; margin: 20px 0; border-radius: 4px; }}
        .ticket-number {{ font-size: 24px; font-weight: bold; color: #d32f2f; font-family: 'Courier New', monospace; }}
        .info-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin: 20px 0; }}
        .info-label {{ font-size: 12px; color: #666; text-transform: uppercase; margin-bottom: 5px; }}
        .info-value {{ font-size: 16px; font-weight: bold; color: #333; }}
        .refund-box {{ background: #e8f5e9; border: 2px solid #4caf50; padding: 20px; margin: 20px 0; border-radius: 8px; text-align: center; }}
        .refund-amount {{ font-size: 32px; font-weight: bold; color: #2e7d32; }}
        .refund-label {{ font-size: 14px; color: #666; margin-top: 5px; }}
        .footer {{ background: #f5f5f5; padding: 20px; text-align: center; border-radius: 0 0 8px 8px; font-size: 14px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Booking Cancelled</h1>
            <p>Your ticket has been successfully cancelled</p>
        </div>
        
        <div class='content'>
            <div class='cancelled-badge'>✗ BOOKING CANCELLED</div>
            
            <div class='ticket-box'>
                <div class='info-label'>Ticket Number</div>
                <div class='ticket-number'>{notification.TicketNumber}</div>
            </div>

            <div class='info-grid'>
                <div class='info-item'>
                    <div class='info-label'>Passenger Name</div>
                    <div class='info-value'>{notification.PassengerName}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Company</div>
                    <div class='info-value'>{notification.CompanyName}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Route</div>
                    <div class='info-value'>{notification.FromCity} → {notification.ToCity}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Journey Date</div>
                    <div class='info-value'>{notification.JourneyDate:dd MMM yyyy}</div>
                </div>
            </div>

            {(notification.RefundAmount > 0 ? $@"
            <div class='refund-box'>
                <div class='refund-label'>Refund Amount</div>
                <div class='refund-amount'>{notification.Currency} {notification.RefundAmount:N2}</div>
                <p style='margin: 10px 0 0 0; color: #666;'>
                    Your refund will be processed within 5-7 business days
                </p>
            </div>
            " : $@"
            <div style='background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0;'>
                <strong style='color: #856404;'>No Refund Available</strong>
                <p style='margin: 10px 0 0 0; color: #666;'>
                    Cancellation was made less than 2 hours before departure
                </p>
            </div>
            ")}

            {(!string.IsNullOrEmpty(notification.CancellationReason) ? $@"
            <div style='background: #f5f5f5; padding: 15px; margin: 20px 0; border-radius: 4px;'>
                <div class='info-label'>Cancellation Reason</div>
                <p style='margin: 5px 0 0 0;'>{notification.CancellationReason}</p>
            </div>
            " : "")}

            <div style='background: #e3f2fd; border-left: 4px solid #2196f3; padding: 15px; margin: 20px 0;'>
                <strong style='color: #1565c0;'>ℹ️ What's Next?</strong>
                <ul style='margin: 10px 0; padding-left: 20px; color: #666;'>
                    <li>Cancellation confirmation has been sent to your phone</li>
                    <li>If applicable, refund will be processed automatically</li>
                    <li>You can book a new ticket anytime from our website</li>
                </ul>
            </div>
        </div>

        <div class='footer'>
            <p style='margin: 0 0 10px 0;'>We're sorry to see you cancel your journey</p>
            <p style='margin: 0; font-size: 12px;'>Hope to serve you again soon! 🚌</p>
            <p style='margin: 15px 0 0 0; font-size: 12px; color: #999;'>
                © 2025 Bus Ticket Reservation System. All rights reserved.
            </p>
        </div>
    </div>
</body>
</html>";
    }

    private string GenerateJourneyReminderHtml(BookingNotificationDto notification, int hoursBeforeDeparture)
    {
        var urgency = hoursBeforeDeparture == 1 ? "URGENT" : "";
        var reminderText = hoursBeforeDeparture == 24 
            ? "Your journey is tomorrow!" 
            : $"Your journey starts in {hoursBeforeDeparture} hour!";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #ff9800 0%, #f57c00 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
        .header h1 {{ margin: 0; font-size: 28px; }}
        .header p {{ margin: 10px 0 0 0; font-size: 16px; opacity: 0.9; }}
        .content {{ background: #fff; padding: 30px; border: 1px solid #e0e0e0; }}
        .reminder-badge {{ background: {(hoursBeforeDeparture == 1 ? "#f44336" : "#ff9800")}; color: white; padding: 8px 20px; border-radius: 20px; display: inline-block; margin: 20px 0; font-weight: bold; }}
        .ticket-box {{ background: #fff8e1; border-left: 4px solid #ff9800; padding: 20px; margin: 20px 0; border-radius: 4px; }}
        .ticket-number {{ font-size: 24px; font-weight: bold; color: #f57c00; font-family: 'Courier New', monospace; }}
        .countdown-box {{ background: linear-gradient(135deg, #ff9800 0%, #f57c00 100%); color: white; padding: 30px; margin: 20px 0; border-radius: 8px; text-align: center; }}
        .countdown-time {{ font-size: 48px; font-weight: bold; margin: 10px 0; }}
        .countdown-label {{ font-size: 16px; opacity: 0.9; }}
        .info-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin: 20px 0; }}
        .info-label {{ font-size: 12px; color: #666; text-transform: uppercase; margin-bottom: 5px; }}
        .info-value {{ font-size: 16px; font-weight: bold; color: #333; }}
        .route-section {{ background: white; border: 2px solid #ff9800; padding: 20px; margin: 20px 0; border-radius: 8px; }}
        .checklist {{ background: #e8f5e9; border-left: 4px solid #4caf50; padding: 20px; margin: 20px 0; border-radius: 4px; }}
        .checklist li {{ margin: 10px 0; font-size: 15px; }}
        .footer {{ background: #f5f5f5; padding: 20px; text-align: center; border-radius: 0 0 8px 8px; font-size: 14px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>⏰ Journey Reminder {urgency}</h1>
            <p>{reminderText}</p>
        </div>
        
        <div class='content'>
            <div class='reminder-badge'>{(hoursBeforeDeparture == 1 ? "🔴 DEPARTURE IN 1 HOUR" : "📅 JOURNEY TOMORROW")}</div>
            
            <div class='countdown-box'>
                <div class='countdown-label'>Time Until Departure</div>
                <div class='countdown-time'>{hoursBeforeDeparture}h</div>
                <div class='countdown-label'>Please arrive 15 minutes early</div>
            </div>

            <div class='ticket-box'>
                <div class='info-label'>Ticket Number</div>
                <div class='ticket-number'>{notification.TicketNumber}</div>
            </div>

            <h2 style='color: #ff9800; border-bottom: 2px solid #ff9800; padding-bottom: 10px;'>Journey Details</h2>
            
            <div class='info-grid'>
                <div class='info-item'>
                    <div class='info-label'>From</div>
                    <div class='info-value'>{notification.FromCity}</div>
                    <div style='font-size: 12px; color: #666; margin-top: 3px;'>{notification.BoardingPoint}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>To</div>
                    <div class='info-value'>{notification.ToCity}</div>
                    <div style='font-size: 12px; color: #666; margin-top: 3px;'>{notification.DroppingPoint}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Departure Date</div>
                    <div class='info-value'>{notification.JourneyDate:dd MMM yyyy}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Departure Time</div>
                    <div class='info-value' style='color: #f44336;'>{notification.DepartureTime}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Company</div>
                    <div class='info-value'>{notification.CompanyName}</div>
                </div>
                <div class='info-item'>
                    <div class='info-label'>Seat Number</div>
                    <div class='info-value'>{notification.SeatNumber}</div>
                </div>
            </div>

            <div class='checklist'>
                <strong style='color: #2e7d32; font-size: 18px;'>✓ Pre-Journey Checklist</strong>
                <ul style='margin: 15px 0; padding-left: 20px;'>
                    <li>✓ Ticket number: <strong>{notification.TicketNumber}</strong></li>
                    <li>✓ Valid ID card (NID/Passport/Driving License)</li>
                    <li>✓ Arrive at <strong>{notification.BoardingPoint}</strong> by <strong>{notification.DepartureTime}</strong></li>
                    <li>✓ Keep your phone charged for ticket verification</li>
                    <li>✓ Check weather and traffic conditions</li>
                    {(hoursBeforeDeparture == 1 ? "<li style='color: #d32f2f;'>⚠️ <strong>Leave for boarding point NOW!</strong></li>" : "")}
                </ul>
            </div>

            <div style='background: #e3f2fd; border-left: 4px solid #2196f3; padding: 15px; margin: 20px 0; border-radius: 4px;'>
                <strong style='color: #1565c0;'>ℹ️ Need Help?</strong>
                <p style='margin: 10px 0 0 0; color: #666;'>
                    Contact: {notification.PhoneNumber} | support@busticket.com
                </p>
            </div>
        </div>

        <div class='footer'>
            <p style='margin: 0 0 10px 0;'>Wishing you a safe and pleasant journey! 🚌</p>
            <p style='margin: 0; font-size: 12px;'>From {notification.CompanyName}</p>
            <p style='margin: 15px 0 0 0; font-size: 12px; color: #999;'>
                © 2025 Bus Ticket Reservation System. All rights reserved.
            </p>
        </div>
    </div>
</body>
</html>";
    }
}

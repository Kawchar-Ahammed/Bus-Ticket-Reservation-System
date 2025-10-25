using BusTicket.Application.Contracts.DTOs;
using BusTicket.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicket.WebApi.Controllers;

/// <summary>
/// Test controller for testing notification system
/// NOTE: Remove this controller in production!
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotificationTestController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<NotificationTestController> _logger;

    public NotificationTestController(
        IEmailService emailService,
        ISmsService smsService,
        ILogger<NotificationTestController> logger)
    {
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    /// <summary>
    /// Test booking confirmation notification
    /// </summary>
    [HttpPost("booking-confirmation")]
    public async Task<IActionResult> TestBookingConfirmation([FromBody] TestNotificationRequest request)
    {
        _logger.LogInformation("Testing booking confirmation notification to {Email}", request.Email);

        try
        {
            var notification = new BookingNotificationDto
            {
                TicketNumber = "TKT-TEST-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                PassengerName = request.PassengerName ?? "Test Passenger",
                Email = request.Email,
                PhoneNumber = request.PhoneNumber ?? "01234567890",
                CompanyName = "Green Line Paribahan",
                BusName = "Volvo AC",
                FromCity = "Dhaka",
                ToCity = "Chittagong",
                JourneyDate = DateTime.Now.AddDays(1),
                DepartureTime = "22:00",
                SeatNumber = "A1",
                BoardingPoint = "Gabtali",
                DroppingPoint = "GEC Circle",
                Fare = 1200.00m,
                Currency = "BDT"
            };

            // Send email
            await _emailService.SendBookingConfirmationAsync(notification, CancellationToken.None);
            
            // Send SMS
            await _smsService.SendBookingConfirmationSmsAsync(notification, CancellationToken.None);

            _logger.LogInformation("Test booking confirmation sent successfully");

            return Ok(new
            {
                Success = true,
                Message = "Booking confirmation notification sent successfully!",
                TicketNumber = notification.TicketNumber,
                SentTo = request.Email,
                Note = "Check your email inbox and API logs for SMS message"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send test booking confirmation");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to send notification",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Test cancellation confirmation notification
    /// </summary>
    [HttpPost("cancellation-confirmation")]
    public async Task<IActionResult> TestCancellationConfirmation([FromBody] TestNotificationRequest request)
    {
        _logger.LogInformation("Testing cancellation confirmation notification to {Email}", request.Email);

        try
        {
            var notification = new CancellationNotificationDto
            {
                TicketNumber = "TKT-TEST-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                PassengerName = request.PassengerName ?? "Test Passenger",
                Email = request.Email,
                PhoneNumber = request.PhoneNumber ?? "01234567890",
                CompanyName = "Green Line Paribahan",
                FromCity = "Dhaka",
                ToCity = "Chittagong",
                JourneyDate = DateTime.Now.AddDays(1),
                RefundAmount = 1200.00m,
                Currency = "BDT",
                CancellationReason = "Test cancellation - Change of plans"
            };

            // Send email
            await _emailService.SendCancellationConfirmationAsync(notification, CancellationToken.None);
            
            // Send SMS
            await _smsService.SendCancellationConfirmationSmsAsync(notification, CancellationToken.None);

            _logger.LogInformation("Test cancellation confirmation sent successfully");

            return Ok(new
            {
                Success = true,
                Message = "Cancellation confirmation notification sent successfully!",
                TicketNumber = notification.TicketNumber,
                SentTo = request.Email,
                RefundAmount = notification.RefundAmount,
                Note = "Check your email inbox and API logs for SMS message"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send test cancellation confirmation");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to send notification",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Test 24-hour journey reminder
    /// </summary>
    [HttpPost("reminder-24h")]
    public async Task<IActionResult> Test24HourReminder([FromBody] TestNotificationRequest request)
    {
        _logger.LogInformation("Testing 24-hour reminder notification to {Email}", request.Email);

        try
        {
            var notification = new BookingNotificationDto
            {
                TicketNumber = "TKT-TEST-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                PassengerName = request.PassengerName ?? "Test Passenger",
                Email = request.Email,
                PhoneNumber = request.PhoneNumber ?? "01234567890",
                CompanyName = "Green Line Paribahan",
                BusName = "Volvo AC",
                FromCity = "Dhaka",
                ToCity = "Chittagong",
                JourneyDate = DateTime.Now.AddDays(1),
                DepartureTime = "22:00",
                SeatNumber = "A1",
                BoardingPoint = "Gabtali",
                DroppingPoint = "GEC Circle",
                Fare = 1200.00m,
                Currency = "BDT"
            };

            // Send email
            await _emailService.SendJourneyReminderAsync(notification, 24, CancellationToken.None);
            
            // Send SMS
            await _smsService.SendJourneyReminderSmsAsync(notification, 24, CancellationToken.None);

            _logger.LogInformation("Test 24-hour reminder sent successfully");

            return Ok(new
            {
                Success = true,
                Message = "24-hour journey reminder sent successfully!",
                TicketNumber = notification.TicketNumber,
                SentTo = request.Email,
                JourneyDate = notification.JourneyDate.ToString("yyyy-MM-dd"),
                Note = "Check your email inbox and API logs for SMS message"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send test 24-hour reminder");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to send notification",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Test 1-hour journey reminder (URGENT)
    /// </summary>
    [HttpPost("reminder-1h")]
    public async Task<IActionResult> Test1HourReminder([FromBody] TestNotificationRequest request)
    {
        _logger.LogInformation("Testing 1-hour URGENT reminder notification to {Email}", request.Email);

        try
        {
            var notification = new BookingNotificationDto
            {
                TicketNumber = "TKT-TEST-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                PassengerName = request.PassengerName ?? "Test Passenger",
                Email = request.Email,
                PhoneNumber = request.PhoneNumber ?? "01234567890",
                CompanyName = "Green Line Paribahan",
                BusName = "Volvo AC",
                FromCity = "Dhaka",
                ToCity = "Chittagong",
                JourneyDate = DateTime.Now,
                DepartureTime = DateTime.Now.AddHours(1).ToString("HH:mm"),
                SeatNumber = "A1",
                BoardingPoint = "Gabtali",
                DroppingPoint = "GEC Circle",
                Fare = 1200.00m,
                Currency = "BDT"
            };

            // Send email
            await _emailService.SendJourneyReminderAsync(notification, 1, CancellationToken.None);
            
            // Send SMS
            await _smsService.SendJourneyReminderSmsAsync(notification, 1, CancellationToken.None);

            _logger.LogInformation("Test 1-hour URGENT reminder sent successfully");

            return Ok(new
            {
                Success = true,
                Message = "1-hour URGENT journey reminder sent successfully!",
                TicketNumber = notification.TicketNumber,
                SentTo = request.Email,
                DepartureTime = notification.DepartureTime,
                Note = "Check your email inbox and API logs for SMS message. This is the URGENT reminder!"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send test 1-hour reminder");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to send notification",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Test all notification types at once
    /// </summary>
    [HttpPost("test-all")]
    public async Task<IActionResult> TestAllNotifications([FromBody] TestNotificationRequest request)
    {
        _logger.LogInformation("Testing ALL notifications to {Email}", request.Email);

        var results = new List<string>();

        try
        {
            // 1. Booking Confirmation
            results.Add("✅ Sending booking confirmation...");
            await TestBookingConfirmationInternal(request);
            results.Add("✅ Booking confirmation sent!");

            // Wait 2 seconds between emails
            await Task.Delay(2000);

            // 2. 24-hour Reminder
            results.Add("✅ Sending 24-hour reminder...");
            await Test24HourReminderInternal(request);
            results.Add("✅ 24-hour reminder sent!");

            await Task.Delay(2000);

            // 3. 1-hour Reminder
            results.Add("✅ Sending 1-hour URGENT reminder...");
            await Test1HourReminderInternal(request);
            results.Add("✅ 1-hour reminder sent!");

            await Task.Delay(2000);

            // 4. Cancellation
            results.Add("✅ Sending cancellation confirmation...");
            await TestCancellationConfirmationInternal(request);
            results.Add("✅ Cancellation confirmation sent!");

            return Ok(new
            {
                Success = true,
                Message = "All notifications sent successfully!",
                SentTo = request.Email,
                Results = results,
                Note = "Check your email inbox for 4 different emails. Also check API logs for SMS messages."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send all test notifications");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to send some notifications",
                Error = ex.Message,
                CompletedSteps = results
            });
        }
    }

    #region Private Helper Methods

    private async Task TestBookingConfirmationInternal(TestNotificationRequest request)
    {
        var notification = new BookingNotificationDto
        {
            TicketNumber = "TKT-BOOKING-" + DateTime.Now.ToString("HHmmss"),
            PassengerName = request.PassengerName ?? "Test Passenger",
            Email = request.Email,
            PhoneNumber = request.PhoneNumber ?? "01234567890",
            CompanyName = "Green Line Paribahan",
            BusName = "Volvo AC",
            FromCity = "Dhaka",
            ToCity = "Chittagong",
            JourneyDate = DateTime.Now.AddDays(1),
            DepartureTime = "22:00",
            SeatNumber = "A1",
            BoardingPoint = "Gabtali",
            DroppingPoint = "GEC Circle",
            Fare = 1200.00m,
            Currency = "BDT"
        };

        await _emailService.SendBookingConfirmationAsync(notification, CancellationToken.None);
        await _smsService.SendBookingConfirmationSmsAsync(notification, CancellationToken.None);
    }

    private async Task Test24HourReminderInternal(TestNotificationRequest request)
    {
        var notification = new BookingNotificationDto
        {
            TicketNumber = "TKT-24H-" + DateTime.Now.ToString("HHmmss"),
            PassengerName = request.PassengerName ?? "Test Passenger",
            Email = request.Email,
            PhoneNumber = request.PhoneNumber ?? "01234567890",
            CompanyName = "Green Line Paribahan",
            BusName = "Volvo AC",
            FromCity = "Dhaka",
            ToCity = "Chittagong",
            JourneyDate = DateTime.Now.AddDays(1),
            DepartureTime = "22:00",
            SeatNumber = "A1",
            BoardingPoint = "Gabtali",
            DroppingPoint = "GEC Circle",
            Fare = 1200.00m,
            Currency = "BDT"
        };

        await _emailService.SendJourneyReminderAsync(notification, 24, CancellationToken.None);
        await _smsService.SendJourneyReminderSmsAsync(notification, 24, CancellationToken.None);
    }

    private async Task Test1HourReminderInternal(TestNotificationRequest request)
    {
        var notification = new BookingNotificationDto
        {
            TicketNumber = "TKT-1H-" + DateTime.Now.ToString("HHmmss"),
            PassengerName = request.PassengerName ?? "Test Passenger",
            Email = request.Email,
            PhoneNumber = request.PhoneNumber ?? "01234567890",
            CompanyName = "Green Line Paribahan",
            BusName = "Volvo AC",
            FromCity = "Dhaka",
            ToCity = "Chittagong",
            JourneyDate = DateTime.Now,
            DepartureTime = DateTime.Now.AddHours(1).ToString("HH:mm"),
            SeatNumber = "A1",
            BoardingPoint = "Gabtali",
            DroppingPoint = "GEC Circle",
            Fare = 1200.00m,
            Currency = "BDT"
        };

        await _emailService.SendJourneyReminderAsync(notification, 1, CancellationToken.None);
        await _smsService.SendJourneyReminderSmsAsync(notification, 1, CancellationToken.None);
    }

    private async Task TestCancellationConfirmationInternal(TestNotificationRequest request)
    {
        var notification = new CancellationNotificationDto
        {
            TicketNumber = "TKT-CANCEL-" + DateTime.Now.ToString("HHmmss"),
            PassengerName = request.PassengerName ?? "Test Passenger",
            Email = request.Email,
            PhoneNumber = request.PhoneNumber ?? "01234567890",
            CompanyName = "Green Line Paribahan",
            FromCity = "Dhaka",
            ToCity = "Chittagong",
            JourneyDate = DateTime.Now.AddDays(1),
            RefundAmount = 1200.00m,
            Currency = "BDT",
            CancellationReason = "Test cancellation"
        };

        await _emailService.SendCancellationConfirmationAsync(notification, CancellationToken.None);
        await _smsService.SendCancellationConfirmationSmsAsync(notification, CancellationToken.None);
    }

    #endregion
}

/// <summary>
/// Request model for testing notifications
/// </summary>
public class TestNotificationRequest
{
    /// <summary>
    /// Email address to send test notification to
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Optional: Passenger name for the test
    /// </summary>
    public string? PassengerName { get; set; }

    /// <summary>
    /// Optional: Phone number for SMS test
    /// </summary>
    public string? PhoneNumber { get; set; }
}

using BusTicket.Application.Contracts.DTOs;
using MediatR;

namespace BusTicket.Application.Queries;

/// <summary>
/// Query to get all bookings for admin (with optional filters)
/// </summary>
public class GetAllBookingsForAdminQuery : IRequest<List<AdminBookingDto>>
{
    /// <summary>
    /// Filter by company ID
    /// </summary>
    public Guid? CompanyId { get; set; }
    
    /// <summary>
    /// Filter by booking status (Confirmed, Cancelled, Completed)
    /// </summary>
    public string? BookingStatus { get; set; }
    
    /// <summary>
    /// Filter by payment status (Pending, Paid, Refunded)
    /// </summary>
    public string? PaymentStatus { get; set; }
    
    /// <summary>
    /// Filter by journey date from
    /// </summary>
    public DateTime? JourneyDateFrom { get; set; }
    
    /// <summary>
    /// Filter by journey date to
    /// </summary>
    public DateTime? JourneyDateTo { get; set; }
    
    /// <summary>
    /// Filter by booking date from
    /// </summary>
    public DateTime? BookingDateFrom { get; set; }
    
    /// <summary>
    /// Filter by booking date to
    /// </summary>
    public DateTime? BookingDateTo { get; set; }
    
    /// <summary>
    /// Search by passenger name, phone number, or ticket number
    /// </summary>
    public string? SearchTerm { get; set; }
}

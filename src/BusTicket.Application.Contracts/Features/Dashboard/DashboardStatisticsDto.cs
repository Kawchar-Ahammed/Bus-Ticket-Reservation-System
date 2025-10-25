namespace BusTicket.Application.Contracts.Features.Dashboard;

public class DashboardStatisticsDto
{
    public int TotalCompanies { get; set; }
    public int ActiveCompanies { get; set; }
    public int TotalBuses { get; set; }
    public int ActiveBuses { get; set; }
    public int TotalRoutes { get; set; }
    public int ActiveRoutes { get; set; }
    public int TotalSchedules { get; set; }
    public int TodaySchedules { get; set; }
    public int TotalBookings { get; set; }
    public int TodayBookings { get; set; }
    public int WeekBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int PendingBookings { get; set; }
    public int CancelledBookings { get; set; }
    public List<RecentBookingDto> RecentBookings { get; set; } = new();
}

public class RecentBookingDto
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public decimal FareAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

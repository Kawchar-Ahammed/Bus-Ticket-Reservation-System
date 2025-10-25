using BusTicket.Application.Contracts.Features.Dashboard;
using BusTicket.Application.Contracts.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BusTicket.Application.Features.Dashboard.Queries.GetDashboardStatistics;

public class GetDashboardStatisticsQueryHandler : IRequestHandler<GetDashboardStatisticsQuery, DashboardStatisticsDto>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IBusRepository _busRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IBusScheduleRepository _scheduleRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ILogger<GetDashboardStatisticsQueryHandler> _logger;

    public GetDashboardStatisticsQueryHandler(
        ICompanyRepository companyRepository,
        IBusRepository busRepository,
        IRouteRepository routeRepository,
        IBusScheduleRepository scheduleRepository,
        ITicketRepository ticketRepository,
        ILogger<GetDashboardStatisticsQueryHandler> logger)
    {
        _companyRepository = companyRepository;
        _busRepository = busRepository;
        _routeRepository = routeRepository;
        _scheduleRepository = scheduleRepository;
        _ticketRepository = ticketRepository;
        _logger = logger;
    }

    public async Task<DashboardStatisticsDto> Handle(GetDashboardStatisticsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching dashboard statistics");

        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);

        // Fetch all data
        var companies = await _companyRepository.GetAllAsync();
        var buses = await _busRepository.GetAllAsync();
        var routes = await _routeRepository.GetAllAsync();
        var schedules = await _scheduleRepository.GetAllAsync();
        var tickets = await _ticketRepository.GetAllTicketsForAdminAsync(cancellationToken: cancellationToken);

        // Calculate statistics
        var statistics = new DashboardStatisticsDto
        {
            // Companies
            TotalCompanies = companies.Count,
            ActiveCompanies = companies.Count(c => c.IsActive),

            // Buses
            TotalBuses = buses.Count,
            ActiveBuses = buses.Count(b => b.IsActive),

            // Routes
            TotalRoutes = routes.Count,
            ActiveRoutes = routes.Count(r => r.IsActive),

            // Schedules
            TotalSchedules = schedules.Count,
            TodaySchedules = schedules.Count(s => s.JourneyDate.Date == today && s.IsActive),

            // Bookings
            TotalBookings = tickets.Count,
            TodayBookings = tickets.Count(t => t.BookingDate.Date == today),
            WeekBookings = tickets.Count(t => t.BookingDate.Date >= weekStart && t.BookingDate.Date <= today),
            ConfirmedBookings = tickets.Count(t => t.IsConfirmed && !t.IsCancelled),
            PendingBookings = tickets.Count(t => !t.IsConfirmed && !t.IsCancelled),
            CancelledBookings = tickets.Count(t => t.IsCancelled),

            // Recent bookings (last 10)
            RecentBookings = tickets
                .Where(t => t.Passenger != null && t.BusSchedule != null)
                .OrderByDescending(t => t.BookingDate)
                .Take(10)
                .Select(t => new RecentBookingDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    PassengerName = t.Passenger!.Name,
                    CompanyName = t.BusSchedule!.Bus.Company.Name,
                    Route = $"{t.BusSchedule.Route.FromLocation.City} → {t.BusSchedule.Route.ToLocation.City}",
                    BookingDate = t.BookingDate,
                    FareAmount = t.Fare.Amount,
                    Status = t.IsCancelled ? "Cancelled" : (t.IsConfirmed ? "Confirmed" : "Pending")
                })
                .ToList()
        };

        _logger.LogInformation("Dashboard statistics fetched successfully");
        return statistics;
    }
}

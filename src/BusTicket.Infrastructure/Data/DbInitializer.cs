using BusTicket.Application.Contracts.Services;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using BusTicket.Domain.ValueObjects;
using BusTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusTicket.Infrastructure.Data;

/// <summary>
/// Database initializer for seeding initial data
/// </summary>
public class DbInitializer
{
    private readonly BusTicketDbContext _context;
    private readonly ILogger<DbInitializer> _logger;
    private readonly IPasswordHasher _passwordHasher;

    public DbInitializer(
        BusTicketDbContext context,
        ILogger<DbInitializer> logger,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Initialize database with migrations and seed data
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // Apply pending migrations
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                _logger.LogInformation("Applying pending migrations...");
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Migrations applied successfully.");
            }

            // Seed initial data if database is empty
            if (!await _context.Companies.AnyAsync())
            {
                _logger.LogInformation("Seeding initial data...");
                await SeedDataAsync();
                _logger.LogInformation("Initial data seeded successfully.");
            }

            // Seed default admin user if not exists
            if (!await _context.AdminUsers.AnyAsync())
            {
                _logger.LogInformation("Seeding default admin user...");
                await SeedAdminUserAsync();
                _logger.LogInformation("Default admin user seeded successfully.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    /// <summary>
    /// Seed initial demo data
    /// </summary>
    private async Task SeedDataAsync()
    {
        // Create Companies
        var company1 = Company.Create(
            name: "Green Line Paribahan",
            description: "Premium bus service provider",
            contactNumber: "+8801711111111",
            email: "greenline@example.com");

        var company2 = Company.Create(
            name: "Hanif Enterprise",
            description: "Trusted transport service",
            contactNumber: "+8801722222222",
            email: "hanif@example.com");

        await _context.Companies.AddRangeAsync(company1, company2);
        await _context.SaveChangesAsync();

        // Create Buses
        var bus1 = Bus.Create(
            busNumber: "DHA-GA-11-2345",
            busName: "AC Sleeper",
            companyId: company1.Id,
            totalSeats: 40,
            description: "Comfortable AC sleeper coach");

        var bus2 = Bus.Create(
            busNumber: "DHA-KA-22-6789",
            busName: "AC Chair Coach",
            companyId: company2.Id,
            totalSeats: 45,
            description: "Modern AC chair coach");

        await _context.Buses.AddRangeAsync(bus1, bus2);
        await _context.SaveChangesAsync();

        // Create Routes
        var route1 = Route.Create(
            fromLocation: Address.Create("Dhaka", "Gabtali"),
            toLocation: Address.Create("Chittagong", "Oxygen"),
            distanceInKm: 264,
            estimatedDuration: TimeSpan.FromHours(6));

        var route2 = Route.Create(
            fromLocation: Address.Create("Dhaka", "Kalyanpur"),
            toLocation: Address.Create("Cox's Bazar", "Kolatoli"),
            distanceInKm: 397,
            estimatedDuration: TimeSpan.FromHours(8));

        var route3 = Route.Create(
            fromLocation: Address.Create("Dhaka", "Mohakhali"),
            toLocation: Address.Create("Sylhet", "Ambarkhana"),
            distanceInKm: 244,
            estimatedDuration: TimeSpan.FromHours(5));

        await _context.Routes.AddRangeAsync(route1, route2, route3);
        await _context.SaveChangesAsync();

        // Create Bus Schedules for next 7 days
        var schedules = new List<BusSchedule>();
        var today = DateTime.UtcNow.Date;

        for (int day = 0; day < 7; day++)
        {
            var date = today.AddDays(day);

            // Schedule 1: Dhaka to Chittagong
            var schedule1 = BusSchedule.Create(
                busId: bus1.Id,
                routeId: route1.Id,
                journeyDate: date,
                departureTime: new TimeSpan(22, 0, 0), // 10:00 PM
                arrivalTime: new TimeSpan(4, 0, 0), // 4:00 AM (next day)
                fare: Money.Create(1200, "BDT"),
                boardingPoint: Address.Create("Dhaka", "Gabtali", "Counter Road"),
                droppingPoint: Address.Create("Chittagong", "GEC Circle", "Station Road"));

            // Schedule 2: Dhaka to Cox's Bazar
            var schedule2 = BusSchedule.Create(
                busId: bus2.Id,
                routeId: route2.Id,
                journeyDate: date,
                departureTime: new TimeSpan(21, 30, 0), // 9:30 PM
                arrivalTime: new TimeSpan(5, 30, 0), // 5:30 AM (next day)
                fare: Money.Create(1800, "BDT"),
                boardingPoint: Address.Create("Dhaka", "Kalyanpur", "Terminal Road"),
                droppingPoint: Address.Create("Cox's Bazar", "Kolatoli", "Beach Road"));

            schedules.Add(schedule1);
            schedules.Add(schedule2);
        }

        await _context.BusSchedules.AddRangeAsync(schedules);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {schedules.Count} bus schedules for the next 7 days.");
    }

    /// <summary>
    /// Seed default admin user
    /// </summary>
    private async Task SeedAdminUserAsync()
    {
        // Create default SuperAdmin user
        var passwordHash = _passwordHasher.HashPassword("Admin@123");

        var superAdmin = AdminUser.Create(
            email: "admin@busticket.com",
            passwordHash: passwordHash,
            fullName: "System Administrator",
            role: AdminRole.SuperAdmin);

        superAdmin.Activate(); // Ensure it's active

        await _context.AdminUsers.AddAsync(superAdmin);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Default SuperAdmin created: admin@busticket.com / Admin@123");
    }
}

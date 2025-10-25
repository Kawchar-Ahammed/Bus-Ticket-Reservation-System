using BusTicket.Application.Contracts.Repositories;
using BusTicket.Application.Contracts.Services;
using BusTicket.Domain.Services;
using BusTicket.Infrastructure.Data;
using BusTicket.Infrastructure.Repositories;
using BusTicket.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusTicket.Infrastructure;

/// <summary>
/// Dependency Injection configuration for Infrastructure Layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Infrastructure Layer services, DbContext, and repositories
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext with PostgreSQL
        services.AddDbContext<BusTicketDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(BusTicketDbContext).Assembly.FullName);
                // Note: EnableRetryOnFailure is disabled because we use manual transactions
                // which are incompatible with retry strategies
            });

            // Enable sensitive data logging in development
            var enableSensitiveLogging = configuration["EnableSensitiveDataLogging"];
            if (!string.IsNullOrEmpty(enableSensitiveLogging) && bool.Parse(enableSensitiveLogging))
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IBusScheduleRepository, BusScheduleRepository>();
        services.AddScoped<ISeatRepository, SeatRepository>();
        services.AddScoped<IPassengerRepository, PassengerRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IBusRepository, BusRepository>();
        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Register Domain Services
        services.AddScoped<ISeatBookingDomainService, SeatBookingDomainService>();

        // Register Notification Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<JourneyReminderService>();

        // Register Authentication Services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        // Register Payment Gateway
        services.AddScoped<IPaymentGateway, MockPaymentGateway>();

        return services;
    }
}

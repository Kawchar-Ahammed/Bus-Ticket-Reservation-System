using BusTicket.Application.Behaviors;
using BusTicket.Application.Contracts.Services;
using BusTicket.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BusTicket.Application;

/// <summary>
/// Dependency Injection configuration for Application Layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Application Layer services, MediatR, validators, and behaviors
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR with all handlers from this assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register MediatR Pipeline Behaviors (order matters!)
        // 1. Logging - logs all requests
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        
        // 2. Validation - validates before handlers execute
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Register Application Services
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}

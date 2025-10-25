using Hangfire.Dashboard;

namespace BusTicket.WebApi.Middleware;

/// <summary>
/// Authorization filter for Hangfire Dashboard
/// In development: allows all access
/// In production: should be secured with proper authentication
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // TODO: In production, add proper authentication
        // For now, allow all access in development
        return true;
    }
}

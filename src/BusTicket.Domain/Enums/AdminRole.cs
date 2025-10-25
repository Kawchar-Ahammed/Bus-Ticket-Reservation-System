namespace BusTicket.Domain.Enums;

/// <summary>
/// Admin user roles with different permission levels
/// </summary>
public enum AdminRole
{
    /// <summary>
    /// Full system access - can manage everything including other admins
    /// </summary>
    SuperAdmin = 0,

    /// <summary>
    /// Business operations - can manage companies, buses, routes, schedules, bookings
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Limited operations - can only manage their company's data
    /// </summary>
    Operator = 2
}

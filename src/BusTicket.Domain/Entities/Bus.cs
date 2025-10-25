using BusTicket.Domain.Common;
using BusTicket.Domain.Enums;
using BusTicket.Domain.ValueObjects;

namespace BusTicket.Domain.Entities;

/// <summary>
/// Aggregate Root: Bus entity
/// </summary>
public class Bus : Entity, IAggregateRoot
{
    public string BusNumber { get; private set; }
    public string BusName { get; private set; }
    public Guid CompanyId { get; private set; }
    public int TotalSeats { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public virtual Company Company { get; private set; } = null!;
    public virtual ICollection<BusSchedule> BusSchedules { get; private set; } = new List<BusSchedule>();

    private Bus() { } // EF Core constructor

    private Bus(string busNumber, string busName, Guid companyId, int totalSeats, string? description)
    {
        BusNumber = busNumber;
        BusName = busName;
        CompanyId = companyId;
        TotalSeats = totalSeats;
        Description = description;
        IsActive = true;
    }

    public static Bus Create(string busNumber, string busName, Guid companyId, int totalSeats, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(busNumber))
            throw new ArgumentException("Bus number cannot be empty", nameof(busNumber));

        if (string.IsNullOrWhiteSpace(busName))
            throw new ArgumentException("Bus name cannot be empty", nameof(busName));

        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID cannot be empty", nameof(companyId));

        if (totalSeats <= 0 || totalSeats > 100)
            throw new ArgumentException("Total seats must be between 1 and 100", nameof(totalSeats));

        return new Bus(busNumber, busName, companyId, totalSeats, description);
    }

    public void UpdateDetails(string busName, string? description)
    {
        if (string.IsNullOrWhiteSpace(busName))
            throw new ArgumentException("Bus name cannot be empty", nameof(busName));

        BusName = busName;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

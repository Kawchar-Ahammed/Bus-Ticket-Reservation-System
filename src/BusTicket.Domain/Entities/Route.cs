using BusTicket.Domain.Common;
using BusTicket.Domain.ValueObjects;

namespace BusTicket.Domain.Entities;

/// <summary>
/// Entity: Route (from one city to another)
/// </summary>
public class Route : Entity, IAggregateRoot
{
    public Address FromLocation { get; private set; } = null!;
    public Address ToLocation { get; private set; } = null!;
    public decimal DistanceInKm { get; private set; }
    public TimeSpan EstimatedDuration { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public virtual ICollection<BusSchedule> BusSchedules { get; private set; } = new List<BusSchedule>();

    private Route() { } // EF Core constructor

    private Route(Address fromLocation, Address toLocation, decimal distanceInKm, TimeSpan estimatedDuration)
    {
        FromLocation = fromLocation;
        ToLocation = toLocation;
        DistanceInKm = distanceInKm;
        EstimatedDuration = estimatedDuration;
        IsActive = true;
    }

    public static Route Create(Address fromLocation, Address toLocation, decimal distanceInKm, TimeSpan estimatedDuration)
    {
        if (fromLocation == null)
            throw new ArgumentNullException(nameof(fromLocation));

        if (toLocation == null)
            throw new ArgumentNullException(nameof(toLocation));

        if (fromLocation.City.Equals(toLocation.City, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("From and To locations cannot be the same");

        if (distanceInKm <= 0)
            throw new ArgumentException("Distance must be greater than zero", nameof(distanceInKm));

        if (estimatedDuration <= TimeSpan.Zero)
            throw new ArgumentException("Estimated duration must be greater than zero", nameof(estimatedDuration));

        return new Route(fromLocation, toLocation, distanceInKm, estimatedDuration);
    }

    public void UpdateDetails(decimal distanceInKm, TimeSpan estimatedDuration)
    {
        if (distanceInKm <= 0)
            throw new ArgumentException("Distance must be greater than zero", nameof(distanceInKm));

        if (estimatedDuration <= TimeSpan.Zero)
            throw new ArgumentException("Estimated duration must be greater than zero", nameof(estimatedDuration));

        DistanceInKm = distanceInKm;
        EstimatedDuration = estimatedDuration;
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

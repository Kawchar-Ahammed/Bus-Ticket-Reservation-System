using BusTicket.Domain.Common;
using BusTicket.Domain.ValueObjects;

namespace BusTicket.Domain.Entities;

/// <summary>
/// Aggregate Root: Bus Schedule - represents a specific bus journey on a route
/// </summary>
public class BusSchedule : Entity, IAggregateRoot
{
    public Guid BusId { get; private set; }
    public Guid RouteId { get; private set; }
    public DateTime JourneyDate { get; private set; }
    public TimeSpan DepartureTime { get; private set; }
    public TimeSpan ArrivalTime { get; private set; }
    public Money Fare { get; private set; } = null!;
    public Address BoardingPoint { get; private set; } = null!;
    public Address DroppingPoint { get; private set; } = null!;
    public bool IsActive { get; private set; }

    // Navigation properties
    public virtual Bus Bus { get; private set; } = null!;
    public virtual Route Route { get; private set; } = null!;
    public virtual ICollection<Seat> Seats { get; private set; } = new List<Seat>();
    public virtual ICollection<Ticket> Tickets { get; private set; } = new List<Ticket>();

    private BusSchedule() { } // EF Core constructor

    private BusSchedule(
        Guid busId,
        Guid routeId,
        DateTime journeyDate,
        TimeSpan departureTime,
        TimeSpan arrivalTime,
        Money fare,
        Address boardingPoint,
        Address droppingPoint)
    {
        BusId = busId;
        RouteId = routeId;
        JourneyDate = DateTime.SpecifyKind(journeyDate.Date, DateTimeKind.Utc); // Ensure UTC
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        Fare = fare;
        BoardingPoint = boardingPoint;
        DroppingPoint = droppingPoint;
        IsActive = true;
    }

    public static BusSchedule Create(
        Guid busId,
        Guid routeId,
        DateTime journeyDate,
        TimeSpan departureTime,
        TimeSpan arrivalTime,
        Money fare,
        Address boardingPoint,
        Address droppingPoint)
    {
        if (busId == Guid.Empty)
            throw new ArgumentException("Bus ID cannot be empty", nameof(busId));

        if (routeId == Guid.Empty)
            throw new ArgumentException("Route ID cannot be empty", nameof(routeId));

        if (journeyDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Journey date cannot be in the past", nameof(journeyDate));

        if (departureTime < TimeSpan.Zero || departureTime >= TimeSpan.FromHours(24))
            throw new ArgumentException("Departure time must be between 00:00 and 23:59", nameof(departureTime));

        if (arrivalTime < TimeSpan.Zero || arrivalTime >= TimeSpan.FromHours(24))
            throw new ArgumentException("Arrival time must be between 00:00 and 23:59", nameof(arrivalTime));

        if (fare == null)
            throw new ArgumentNullException(nameof(fare));

        if (boardingPoint == null)
            throw new ArgumentNullException(nameof(boardingPoint));

        if (droppingPoint == null)
            throw new ArgumentNullException(nameof(droppingPoint));

        return new BusSchedule(busId, routeId, journeyDate, departureTime, arrivalTime, fare, boardingPoint, droppingPoint);
    }

    public void UpdateSchedule(DateTime journeyDate, TimeSpan departureTime, TimeSpan arrivalTime, Money fare)
    {
        if (journeyDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Journey date cannot be in the past", nameof(journeyDate));

        JourneyDate = DateTime.SpecifyKind(journeyDate.Date, DateTimeKind.Utc);
        DepartureTime = departureTime;
        ArrivalTime = arrivalTime;
        Fare = fare;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBoardingAndDroppingPoints(Address boardingPoint, Address droppingPoint)
    {
        BoardingPoint = boardingPoint ?? throw new ArgumentNullException(nameof(boardingPoint));
        DroppingPoint = droppingPoint ?? throw new ArgumentNullException(nameof(droppingPoint));
        UpdatedAt = DateTime.UtcNow;
    }

    public int GetAvailableSeatsCount()
    {
        return Seats.Count(s => s.Status == Enums.SeatStatus.Available);
    }

    public int GetBookedSeatsCount()
    {
        return Seats.Count(s => s.Status == Enums.SeatStatus.Booked || s.Status == Enums.SeatStatus.Sold);
    }

    public void Cancel()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

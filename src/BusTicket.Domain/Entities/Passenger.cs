using BusTicket.Domain.Common;
using BusTicket.Domain.Enums;
using BusTicket.Domain.ValueObjects;

namespace BusTicket.Domain.Entities;

/// <summary>
/// Entity: Passenger information
/// </summary>
public class Passenger : Entity
{
    public string Name { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    public string? Email { get; private set; }
    public Gender? Gender { get; private set; }
    public int? Age { get; private set; }

    // Navigation properties
    public virtual ICollection<Ticket> Tickets { get; private set; } = new List<Ticket>();

    private Passenger() { } // EF Core constructor

    private Passenger(string name, PhoneNumber phoneNumber, string? email, Gender? gender, int? age)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        Gender = gender;
        Age = age;
    }

    public static Passenger Create(string name, PhoneNumber phoneNumber, string? email = null, Gender? gender = null, int? age = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Passenger name cannot be empty", nameof(name));

        if (phoneNumber == null)
            throw new ArgumentNullException(nameof(phoneNumber));

        if (age.HasValue && (age.Value < 1 || age.Value > 120))
            throw new ArgumentException("Age must be between 1 and 120", nameof(age));

        return new Passenger(name.Trim(), phoneNumber, email?.Trim(), gender, age);
    }

    public void UpdateDetails(string name, PhoneNumber phoneNumber, string? email, Gender? gender, int? age)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Passenger name cannot be empty", nameof(name));

        if (phoneNumber == null)
            throw new ArgumentNullException(nameof(phoneNumber));

        Name = name.Trim();
        PhoneNumber = phoneNumber;
        Email = email?.Trim();
        Gender = gender;
        Age = age;
        UpdatedAt = DateTime.UtcNow;
    }
}

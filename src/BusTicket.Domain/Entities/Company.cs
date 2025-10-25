using BusTicket.Domain.Common;

namespace BusTicket.Domain.Entities;

/// <summary>
/// Entity: Company (Bus Operator)
/// </summary>
public class Company : Entity, IAggregateRoot
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? ContactNumber { get; private set; }
    public string? Email { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public virtual ICollection<Bus> Buses { get; private set; } = new List<Bus>();

    private Company() { } // EF Core constructor

    private Company(string name, string? description, string? contactNumber, string? email)
    {
        Name = name;
        Description = description;
        ContactNumber = contactNumber;
        Email = email;
        IsActive = true;
    }

    public static Company Create(string name, string? description = null, string? contactNumber = null, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty", nameof(name));

        return new Company(name, description, contactNumber, email);
    }

    public void UpdateDetails(string name, string? description, string? contactNumber, string? email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty", nameof(name));

        Name = name;
        Description = description;
        ContactNumber = contactNumber;
        Email = email;
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

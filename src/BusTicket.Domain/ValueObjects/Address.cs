using BusTicket.Domain.Common;

namespace BusTicket.Domain.ValueObjects;

/// <summary>
/// Value Object representing an Address/Location
/// </summary>
public class Address : ValueObject
{
    public string City { get; private set; }
    public string? Area { get; private set; }
    public string? Street { get; private set; }
    public string? PostalCode { get; private set; }

    private Address(string city, string? area, string? street, string? postalCode)
    {
        City = city;
        Area = area;
        Street = street;
        PostalCode = postalCode;
    }

    public static Address Create(string city, string? area = null, string? street = null, string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));

        return new Address(city.Trim(), area?.Trim(), street?.Trim(), postalCode?.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return City;
        yield return Area;
        yield return Street;
        yield return PostalCode;
    }

    public override string ToString()
    {
        var parts = new List<string> { City };
        if (!string.IsNullOrEmpty(Area)) parts.Add(Area);
        if (!string.IsNullOrEmpty(Street)) parts.Add(Street);
        if (!string.IsNullOrEmpty(PostalCode)) parts.Add(PostalCode);
        return string.Join(", ", parts);
    }
}

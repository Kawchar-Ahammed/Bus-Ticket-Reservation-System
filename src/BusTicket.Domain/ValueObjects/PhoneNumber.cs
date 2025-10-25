using BusTicket.Domain.Common;

namespace BusTicket.Domain.ValueObjects;

/// <summary>
/// Value Object representing a Phone Number
/// </summary>
public class PhoneNumber : ValueObject
{
    public string Value { get; private set; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        // Remove spaces and dashes
        var cleanedNumber = phoneNumber.Replace(" ", "").Replace("-", "");

        // Basic validation (adjust based on your requirements)
        if (cleanedNumber.Length < 10 || cleanedNumber.Length > 15)
            throw new ArgumentException("Phone number must be between 10 and 15 digits", nameof(phoneNumber));

        if (!cleanedNumber.All(char.IsDigit) && !cleanedNumber.StartsWith("+"))
            throw new ArgumentException("Phone number must contain only digits", nameof(phoneNumber));

        return new PhoneNumber(cleanedNumber);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

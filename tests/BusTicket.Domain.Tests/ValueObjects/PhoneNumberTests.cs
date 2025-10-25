using BusTicket.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BusTicket.Domain.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("01712345678")]  // Bangladesh format
    [InlineData("01812345678")]
    [InlineData("01912345678")]
    [InlineData("+8801712345678")]  // With country code
    public void Create_WithValidPhoneNumber_ShouldSucceed(string phoneNumber)
    {
        // Act
        var phone = PhoneNumber.Create(phoneNumber);

        // Assert
        phone.Should().NotBeNull();
        phone.Value.Should().Be(phoneNumber);
    }

    [Theory]
    [InlineData("123")]  // Too short
    [InlineData("")]  // Empty
    [InlineData("12345678901234567890")]  // Too long
    public void Create_WithInvalidPhoneNumber_ShouldThrowException(string phoneNumber)
    {
        // Act
        var act = () => PhoneNumber.Create(phoneNumber);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equals_WithSamePhoneNumber_ShouldReturnTrue()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("01712345678");
        var phone2 = PhoneNumber.Create("01712345678");

        // Act & Assert
        phone1.Should().Be(phone2);
    }

    [Fact]
    public void Equals_WithDifferentPhoneNumber_ShouldReturnFalse()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("01712345678");
        var phone2 = PhoneNumber.Create("01812345678");

        // Act & Assert
        phone1.Should().NotBe(phone2);
    }
}

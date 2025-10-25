using BusTicket.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BusTicket.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldSucceed()
    {
        // Arrange
        var amount = 100.50m;
        var currency = "BDT";

        // Act
        var money = Money.Create(amount, currency);

        // Assert
        money.Should().NotBeNull();
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        var amount = -10m;
        var currency = "BDT";

        // Act
        var act = () => Money.Create(amount, currency);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldSucceed()
    {
        // Arrange
        var amount = 0m;
        var currency = "BDT";

        // Act
        var money = Money.Create(amount, currency);

        // Assert
        money.Amount.Should().Be(0m);
    }

    [Fact]
    public void Create_WithEmptyCurrency_ShouldThrowException()
    {
        // Arrange
        var amount = 100m;
        var currency = "";

        // Act
        var act = () => Money.Create(amount, currency);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*currency*");
    }

    [Fact]
    public void Equals_WithSameAmountAndCurrency_ShouldReturnTrue()
    {
        // Arrange
        var money1 = Money.Create(100m, "BDT");
        var money2 = Money.Create(100m, "BDT");

        // Act & Assert
        money1.Should().Be(money2);
    }

    [Fact]
    public void Equals_WithDifferentAmount_ShouldReturnFalse()
    {
        // Arrange
        var money1 = Money.Create(100m, "BDT");
        var money2 = Money.Create(200m, "BDT");

        // Act & Assert
        money1.Should().NotBe(money2);
    }

    [Fact]
    public void Equals_WithDifferentCurrency_ShouldReturnFalse()
    {
        // Arrange
        var money1 = Money.Create(100m, "BDT");
        var money2 = Money.Create(100m, "USD");

        // Act & Assert
        money1.Should().NotBe(money2);
    }
}

using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using BusTicket.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace BusTicket.Domain.Tests.Entities;

public class SeatTests
{
    #region Create Tests

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();
        var seatNumber = "A1";
        var row = 1;
        var column = 1;

        // Act
        var seat = Seat.Create(busScheduleId, seatNumber, row, column);

        // Assert
        seat.Should().NotBeNull();
        seat.BusScheduleId.Should().Be(busScheduleId);
        seat.SeatNumber.Should().Be(seatNumber);
        seat.Row.Should().Be(row);
        seat.Column.Should().Be(column);
        seat.Status.Should().Be(SeatStatus.Available);
        seat.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithEmptySeatNumber_ShouldThrowException()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();

        // Act
        var act = () => Seat.Create(busScheduleId, "", 1, 1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*seatNumber*");
    }

    [Fact]
    public void Create_WithInvalidRow_ShouldThrowException()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();

        // Act
        var act = () => Seat.Create(busScheduleId, "A1", 0, 1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*row*");
    }

    #endregion

    #region Seat Status Transition Tests (ASSIGNMENT REQUIREMENT)

    [Fact]
    public void Book_WhenSeatIsAvailable_ShouldSucceed()
    {
        // Arrange
        var seat = Seat.Create(Guid.NewGuid(), "A1", 1, 1);
        var ticketId = Guid.NewGuid();
        seat.Status.Should().Be(SeatStatus.Available);

        // Act
        seat.Book(ticketId);

        // Assert
        seat.Status.Should().Be(SeatStatus.Booked);
        seat.TicketId.Should().Be(ticketId);
    }

    [Fact]
    public void Book_WhenSeatIsAlreadyBooked_ShouldThrowException()
    {
        // Arrange
        var seat = Seat.Create(Guid.NewGuid(), "A1", 1, 1);
        seat.Book(Guid.NewGuid());

        // Act
        var act = () => seat.Book(Guid.NewGuid());

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not available*");
    }

    [Fact]
    public void Book_WhenSeatIsSold_ShouldThrowException()
    {
        // Arrange
        var seat = Seat.Create(Guid.NewGuid(), "A1", 1, 1);
        seat.Book(Guid.NewGuid());
        seat.ConfirmBooking();

        // Act
        var act = () => seat.Book(Guid.NewGuid());

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ConfirmBooking_WhenSeatIsBooked_ShouldSucceed()
    {
        // Arrange
        var seat = Seat.Create(Guid.NewGuid(), "A1", 1, 1);
        seat.Book(Guid.NewGuid());

        // Act
        seat.ConfirmBooking();

        // Assert
        seat.Status.Should().Be(SeatStatus.Sold);
    }

    [Fact]
    public void CancelBooking_WhenSeatIsBooked_ShouldSucceed()
    {
        // Arrange
        var seat = Seat.Create(Guid.NewGuid(), "A1", 1, 1);
        seat.Book(Guid.NewGuid());

        // Act
        seat.CancelBooking();

        // Assert
        seat.Status.Should().Be(SeatStatus.Available);
        seat.TicketId.Should().BeNull();
    }

    [Fact]
    public void Block_ShouldChangeStatusToBlocked()
    {
        // Arrange
        var seat = Seat.Create(Guid.NewGuid(), "A1", 1, 1);

        // Act
        seat.Block();

        // Assert
        seat.Status.Should().Be(SeatStatus.Blocked);
    }

    [Fact]
    public void CorrectSeatStatusUpdateAfterCancellation_ShouldReleaseToAvailable()
    {
        // Arrange - Book a seat
        var seat = Seat.Create(Guid.NewGuid(), "A1", 1, 1);
        seat.Book(Guid.NewGuid());
        seat.Status.Should().Be(SeatStatus.Booked);

        // Act - Cancel booking (release seat)
        seat.CancelBooking();

        // Assert - Seat should be available again
        seat.Status.Should().Be(SeatStatus.Available);
    }

    #endregion
}

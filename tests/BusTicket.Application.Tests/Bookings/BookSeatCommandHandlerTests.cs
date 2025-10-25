using BusTicket.Application.Bookings.Commands.BookSeat;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using BusTicket.Domain.Exceptions;
using BusTicket.Domain.Interfaces;
using BusTicket.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace BusTicket.Application.Tests.Bookings;

/// <summary>
/// ASSIGNMENT REQUIREMENT: Unit tests for Booking logic
/// Tests the BookSeatCommandHandler for correct booking behavior and seat availability validation
/// </summary>
public class BookSeatCommandHandlerTests
{
    private readonly Mock<IRepository<Seat>> _seatRepositoryMock;
    private readonly Mock<IRepository<Ticket>> _ticketRepositoryMock;
    private readonly Mock<IRepository<Passenger>> _passengerRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly BookSeatCommandHandler _handler;

    public BookSeatCommandHandlerTests()
    {
        _seatRepositoryMock = new Mock<IRepository<Seat>>();
        _ticketRepositoryMock = new Mock<IRepository<Ticket>>();
        _passengerRepositoryMock = new Mock<IRepository<Passenger>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new BookSeatCommandHandler(
            _seatRepositoryMock.Object,
            _ticketRepositoryMock.Object,
            _passengerRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenSeatIsAvailable_ShouldBookSuccessfully()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var seat = Seat.Create(busScheduleId, "A1", "A", 1);
        typeof(Seat).GetProperty("Id")?.SetValue(seat, seatId);

        var command = new BookSeatCommand
        {
            BusScheduleId = busScheduleId,
            SeatNumbers = new List<string> { "A1" },
            PassengerName = "John Doe",
            PassengerEmail = "john@example.com",
            PassengerPhone = "01712345678",
            PassengerGender = Gender.Male
        };

        _seatRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Seat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Seat> { seat });

        _passengerRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Passenger>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _ticketRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - ASSIGNMENT REQUIREMENT: Successful booking should create ticket and update seat status
        result.Should().NotBeNull();
        result.BookingSuccessful.Should().BeTrue();
        seat.Status.Should().Be(SeatStatus.Booked); // Seat status should be updated

        // Verify passenger was created
        _passengerRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Passenger>(p =>
                p.Name == "John Doe" &&
                p.Email == "john@example.com" &&
                p.Phone.Value == "01712345678" &&
                p.Gender == Gender.Male),
                It.IsAny<CancellationToken>()),
            Times.Once
        );

        // Verify ticket was created
        _ticketRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        // Verify transaction was saved
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenSeatIsAlreadyBooked_ShouldThrowException()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();
        var seatId = Guid.NewGuid();
        var seat = Seat.Create(busScheduleId, "A1", "A", 1);
        seat.MarkAsBooked(); // Pre-book the seat
        typeof(Seat).GetProperty("Id")?.SetValue(seat, seatId);

        var command = new BookSeatCommand
        {
            BusScheduleId = busScheduleId,
            SeatNumbers = new List<string> { "A1" },
            PassengerName = "John Doe",
            PassengerEmail = "john@example.com",
            PassengerPhone = "01712345678",
            PassengerGender = Gender.Male
        };

        _seatRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Seat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Seat> { seat });

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert - ASSIGNMENT REQUIREMENT: Booking already-booked seat should fail with validation
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not available*");

        // Verify no ticket was created
        _ticketRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        // Verify no transaction was saved
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WhenSeatIsSold_ShouldThrowException()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();
        var seat = Seat.Create(busScheduleId, "A1", "A", 1);
        seat.MarkAsBooked();
        seat.MarkAsSold(); // Mark as sold

        var command = new BookSeatCommand
        {
            BusScheduleId = busScheduleId,
            SeatNumbers = new List<string> { "A1" },
            PassengerName = "John Doe",
            PassengerEmail = "john@example.com",
            PassengerPhone = "01712345678",
            PassengerGender = Gender.Male
        };

        _seatRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Seat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Seat> { seat });

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WhenSeatDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();

        var command = new BookSeatCommand
        {
            BusScheduleId = busScheduleId,
            SeatNumbers = new List<string> { "NonExistent" },
            PassengerName = "John Doe",
            PassengerEmail = "john@example.com",
            PassengerPhone = "01712345678",
            PassengerGender = Gender.Male
        };

        _seatRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Seat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Seat>()); // No seats found

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_WithMultipleSeats_ShouldBookAllSeats()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();
        var seat1 = Seat.Create(busScheduleId, "A1", "A", 1);
        var seat2 = Seat.Create(busScheduleId, "A2", "A", 2);

        var command = new BookSeatCommand
        {
            BusScheduleId = busScheduleId,
            SeatNumbers = new List<string> { "A1", "A2" },
            PassengerName = "John Doe",
            PassengerEmail = "john@example.com",
            PassengerPhone = "01712345678",
            PassengerGender = Gender.Male
        };

        _seatRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Seat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Seat> { seat1, seat2 });

        _passengerRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Passenger>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _ticketRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Both seats should be booked
        result.Should().NotBeNull();
        result.BookingSuccessful.Should().BeTrue();
        seat1.Status.Should().Be(SeatStatus.Booked);
        seat2.Status.Should().Be(SeatStatus.Booked);

        // Verify ticket was created for both seats
        _ticketRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }

    [Fact]
    public async Task Handle_WithOneUnavailableSeatInMultipleBooking_ShouldThrowAndNotBookAny()
    {
        // Arrange
        var busScheduleId = Guid.NewGuid();
        var seat1 = Seat.Create(busScheduleId, "A1", "A", 1);
        var seat2 = Seat.Create(busScheduleId, "A2", "A", 2);
        seat2.MarkAsBooked(); // Second seat already booked

        var command = new BookSeatCommand
        {
            BusScheduleId = busScheduleId,
            SeatNumbers = new List<string> { "A1", "A2" },
            PassengerName = "John Doe",
            PassengerEmail = "john@example.com",
            PassengerPhone = "01712345678",
            PassengerGender = Gender.Male
        };

        _seatRepositoryMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Seat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Seat> { seat1, seat2 });

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert - Should fail atomically (all-or-nothing)
        await act.Should().ThrowAsync<InvalidOperationException>();

        // Verify no transaction was committed
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}

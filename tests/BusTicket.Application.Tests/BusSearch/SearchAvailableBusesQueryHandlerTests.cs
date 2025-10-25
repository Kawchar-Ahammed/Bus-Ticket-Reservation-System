using BusTicket.Application.BusSearch.Queries.SearchAvailableBuses;
using BusTicket.Application.Contracts.DTOs;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Enums;
using BusTicket.Domain.Interfaces;
using BusTicket.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace BusTicket.Application.Tests.BusSearch;

/// <summary>
/// ASSIGNMENT REQUIREMENT: Unit tests for Search functionality
/// Tests the SearchAvailableBusesQueryHandler for correct behavior
/// </summary>
public class SearchAvailableBusesQueryHandlerTests
{
    private readonly Mock<IRepository<BusSchedule>> _busScheduleRepositoryMock;
    private readonly Mock<IRepository<Seat>> _seatRepositoryMock;
    private readonly SearchAvailableBusesQueryHandler _handler;

    public SearchAvailableBusesQueryHandlerTests()
    {
        _busScheduleRepositoryMock = new Mock<IRepository<BusSchedule>>();
        _seatRepositoryMock = new Mock<IRepository<Seat>>();
        _handler = new SearchAvailableBusesQueryHandler(
            _busScheduleRepositoryMock.Object,
            _seatRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCriteria_ShouldReturnMatchingBuses()
    {
        // Arrange
        var fromCity = "Dhaka";
        var toCity = "Chittagong";
        var journeyDate = DateTime.Today.AddDays(1);

        var company = Company.Create("Green Line", "GL", "greenline@example.com", "01712345678");
        var route = Route.Create("Dhaka-Chittagong", fromCity, toCity, TimeSpan.FromHours(6), Money.Create(800m, "BDT"));
        var bus = Bus.Create("AC-Deluxe-001", "AC Deluxe", 40, company.Id);

        var schedule = BusSchedule.Create(
            bus.Id,
            route.Id,
            journeyDate.Date.AddHours(8), // 8:00 AM
            journeyDate.Date.AddHours(14), // 2:00 PM
            Money.Create(800m, "BDT")
        );

        // Set up relationships via reflection or constructor
        typeof(BusSchedule).GetProperty("Bus")?.SetValue(schedule, bus);
        typeof(BusSchedule).GetProperty("Route")?.SetValue(schedule, route);
        typeof(Bus).GetProperty("Company")?.SetValue(bus, company);

        var schedules = new List<BusSchedule> { schedule };

        // Create seats for the schedule
        var seats = new List<Seat>();
        for (int i = 1; i <= 40; i++)
        {
            var seat = Seat.Create(schedule.Id, $"A{i}", "A", i);
            seats.Add(seat);
        }

        _busScheduleRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedules);

        _seatRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(seats);

        var query = new SearchAvailableBusesQuery(fromCity, toCity, journeyDate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert - Assignment Requirement: Returns Company Name, Bus Name, Start/Arrival Time, Seats Left, Price
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        
        var busDto = result.First();
        busDto.CompanyName.Should().Be("Green Line");
        busDto.BusName.Should().Be("AC Deluxe");
        busDto.DepartureTime.Should().Be(journeyDate.Date.AddHours(8));
        busDto.ArrivalTime.Should().Be(journeyDate.Date.AddHours(14));
        busDto.AvailableSeats.Should().Be(40);
        busDto.PriceAmount.Should().Be(800m);
        busDto.PriceCurrency.Should().Be("BDT");
    }

    [Fact]
    public async Task Handle_WithNonMatchingRoute_ShouldReturnEmptyList()
    {
        // Arrange
        var fromCity = "Dhaka";
        var toCity = "Sylhet";
        var journeyDate = DateTime.Today.AddDays(1);

        var company = Company.Create("Green Line", "GL", "greenline@example.com", "01712345678");
        var route = Route.Create("Dhaka-Chittagong", "Dhaka", "Chittagong", TimeSpan.FromHours(6), Money.Create(800m, "BDT"));
        var bus = Bus.Create("AC-Deluxe-001", "AC Deluxe", 40, company.Id);

        var schedule = BusSchedule.Create(
            bus.Id,
            route.Id,
            journeyDate.Date.AddHours(8),
            journeyDate.Date.AddHours(14),
            Money.Create(800m, "BDT")
        );

        typeof(BusSchedule).GetProperty("Bus")?.SetValue(schedule, bus);
        typeof(BusSchedule).GetProperty("Route")?.SetValue(schedule, route);
        typeof(Bus).GetProperty("Company")?.SetValue(bus, company);

        var schedules = new List<BusSchedule> { schedule };

        _busScheduleRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedules);

        _seatRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Seat>());

        var query = new SearchAvailableBusesQuery(fromCity, toCity, journeyDate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert - Should return empty list when route doesn't match
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithDifferentDate_ShouldReturnEmptyList()
    {
        // Arrange
        var fromCity = "Dhaka";
        var toCity = "Chittagong";
        var searchDate = DateTime.Today.AddDays(2); // Different date
        var scheduleDate = DateTime.Today.AddDays(1);

        var company = Company.Create("Green Line", "GL", "greenline@example.com", "01712345678");
        var route = Route.Create("Dhaka-Chittagong", fromCity, toCity, TimeSpan.FromHours(6), Money.Create(800m, "BDT"));
        var bus = Bus.Create("AC-Deluxe-001", "AC Deluxe", 40, company.Id);

        var schedule = BusSchedule.Create(
            bus.Id,
            route.Id,
            scheduleDate.Date.AddHours(8),
            scheduleDate.Date.AddHours(14),
            Money.Create(800m, "BDT")
        );

        typeof(BusSchedule).GetProperty("Bus")?.SetValue(schedule, bus);
        typeof(BusSchedule).GetProperty("Route")?.SetValue(schedule, route);
        typeof(Bus).GetProperty("Company")?.SetValue(bus, company);

        var schedules = new List<BusSchedule> { schedule };

        _busScheduleRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedules);

        var query = new SearchAvailableBusesQuery(fromCity, toCity, searchDate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithMultipleMatchingSchedules_ShouldReturnAll()
    {
        // Arrange
        var fromCity = "Dhaka";
        var toCity = "Chittagong";
        var journeyDate = DateTime.Today.AddDays(1);

        var company1 = Company.Create("Green Line", "GL", "greenline@example.com", "01712345678");
        var company2 = Company.Create("Shyamoli", "SH", "shyamoli@example.com", "01812345678");
        var route = Route.Create("Dhaka-Chittagong", fromCity, toCity, TimeSpan.FromHours(6), Money.Create(800m, "BDT"));
        var bus1 = Bus.Create("AC-001", "AC Deluxe", 40, company1.Id);
        var bus2 = Bus.Create("NON-AC-001", "Non-AC", 50, company2.Id);

        var schedule1 = BusSchedule.Create(bus1.Id, route.Id, journeyDate.Date.AddHours(8), journeyDate.Date.AddHours(14), Money.Create(800m, "BDT"));
        var schedule2 = BusSchedule.Create(bus2.Id, route.Id, journeyDate.Date.AddHours(10), journeyDate.Date.AddHours(16), Money.Create(600m, "BDT"));

        typeof(BusSchedule).GetProperty("Bus")?.SetValue(schedule1, bus1);
        typeof(BusSchedule).GetProperty("Route")?.SetValue(schedule1, route);
        typeof(Bus).GetProperty("Company")?.SetValue(bus1, company1);

        typeof(BusSchedule).GetProperty("Bus")?.SetValue(schedule2, bus2);
        typeof(BusSchedule).GetProperty("Route")?.SetValue(schedule2, route);
        typeof(Bus).GetProperty("Company")?.SetValue(bus2, company2);

        var schedules = new List<BusSchedule> { schedule1, schedule2 };

        _busScheduleRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedules);

        _seatRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Seat>());

        var query = new SearchAvailableBusesQuery(fromCity, toCity, journeyDate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(dto => dto.CompanyName == "Green Line");
        result.Should().Contain(dto => dto.CompanyName == "Shyamoli");
    }

    [Fact]
    public async Task Handle_WithSomeSeatsBooked_ShouldReturnCorrectAvailableSeatsCount()
    {
        // Arrange
        var fromCity = "Dhaka";
        var toCity = "Chittagong";
        var journeyDate = DateTime.Today.AddDays(1);

        var company = Company.Create("Green Line", "GL", "greenline@example.com", "01712345678");
        var route = Route.Create("Dhaka-Chittagong", fromCity, toCity, TimeSpan.FromHours(6), Money.Create(800m, "BDT"));
        var bus = Bus.Create("AC-Deluxe-001", "AC Deluxe", 40, company.Id);

        var schedule = BusSchedule.Create(bus.Id, route.Id, journeyDate.Date.AddHours(8), journeyDate.Date.AddHours(14), Money.Create(800m, "BDT"));

        typeof(BusSchedule).GetProperty("Bus")?.SetValue(schedule, bus);
        typeof(BusSchedule).GetProperty("Route")?.SetValue(schedule, route);
        typeof(Bus).GetProperty("Company")?.SetValue(bus, company);

        var schedules = new List<BusSchedule> { schedule };

        // Create 40 seats, book 10 of them
        var seats = new List<Seat>();
        for (int i = 1; i <= 40; i++)
        {
            var seat = Seat.Create(schedule.Id, $"A{i}", "A", i);
            if (i <= 10)
            {
                seat.MarkAsBooked(); // Book first 10 seats
            }
            seats.Add(seat);
        }

        _busScheduleRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedules);

        _seatRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(seats);

        var query = new SearchAvailableBusesQuery(fromCity, toCity, journeyDate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert - Should show 30 available seats (40 total - 10 booked)
        result.Should().HaveCount(1);
        result.First().AvailableSeats.Should().Be(30);
    }
}

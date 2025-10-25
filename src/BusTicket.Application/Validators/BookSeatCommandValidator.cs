using BusTicket.Application.Commands;
using FluentValidation;

namespace BusTicket.Application.Validators;

/// <summary>
/// Validator for BookSeatCommand
/// </summary>
public class BookSeatCommandValidator : AbstractValidator<BookSeatCommand>
{
    public BookSeatCommandValidator()
    {
        RuleFor(x => x.BusScheduleId)
            .NotEmpty().WithMessage("Bus schedule ID is required")
            .NotEqual(Guid.Empty).WithMessage("Bus schedule ID cannot be empty");

        RuleFor(x => x.SeatId)
            .NotEmpty().WithMessage("Seat ID is required")
            .NotEqual(Guid.Empty).WithMessage("Seat ID cannot be empty");

        RuleFor(x => x.PassengerName)
            .NotEmpty().WithMessage("Passenger name is required")
            .MinimumLength(2).WithMessage("Passenger name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Passenger name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s.'-]+$").WithMessage("Passenger name contains invalid characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MinimumLength(10).WithMessage("Phone number must be at least 10 digits")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .Matches(@"^[\d\s\-+()]+$").WithMessage("Phone number contains invalid characters");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.BoardingPoint)
            .NotEmpty().WithMessage("Boarding point is required")
            .MaximumLength(200).WithMessage("Boarding point cannot exceed 200 characters");

        RuleFor(x => x.DroppingPoint)
            .NotEmpty().WithMessage("Dropping point is required")
            .MaximumLength(200).WithMessage("Dropping point cannot exceed 200 characters");

        RuleFor(x => x.Gender)
            .Must(g => g == null || new[] { "Male", "Female", "Other" }.Contains(g))
            .WithMessage("Gender must be Male, Female, or Other")
            .When(x => !string.IsNullOrWhiteSpace(x.Gender));

        RuleFor(x => x.Age)
            .InclusiveBetween(1, 120).WithMessage("Age must be between 1 and 120")
            .When(x => x.Age.HasValue);
    }
}

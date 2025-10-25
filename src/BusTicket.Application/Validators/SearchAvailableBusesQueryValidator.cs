using BusTicket.Application.Queries;
using FluentValidation;

namespace BusTicket.Application.Validators;

/// <summary>
/// Validator for SearchAvailableBusesQuery
/// </summary>
public class SearchAvailableBusesQueryValidator : AbstractValidator<SearchAvailableBusesQuery>
{
    public SearchAvailableBusesQueryValidator()
    {
        RuleFor(x => x.From)
            .NotEmpty().WithMessage("Departure city is required")
            .MinimumLength(2).WithMessage("Departure city must be at least 2 characters")
            .MaximumLength(100).WithMessage("Departure city cannot exceed 100 characters");

        RuleFor(x => x.To)
            .NotEmpty().WithMessage("Destination city is required")
            .MinimumLength(2).WithMessage("Destination city must be at least 2 characters")
            .MaximumLength(100).WithMessage("Destination city cannot exceed 100 characters");

        RuleFor(x => x)
            .Must(x => !x.From.Equals(x.To, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Departure and destination cities cannot be the same");

        RuleFor(x => x.JourneyDate)
            .NotEmpty().WithMessage("Journey date is required")
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("Journey date cannot be in the past");
    }
}

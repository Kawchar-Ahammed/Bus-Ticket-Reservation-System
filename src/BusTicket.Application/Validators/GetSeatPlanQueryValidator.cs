using BusTicket.Application.Queries;
using FluentValidation;

namespace BusTicket.Application.Validators;

/// <summary>
/// Validator for GetSeatPlanQuery
/// </summary>
public class GetSeatPlanQueryValidator : AbstractValidator<GetSeatPlanQuery>
{
    public GetSeatPlanQueryValidator()
    {
        RuleFor(x => x.BusScheduleId)
            .NotEmpty().WithMessage("Bus schedule ID is required")
            .NotEqual(Guid.Empty).WithMessage("Bus schedule ID cannot be empty");
    }
}

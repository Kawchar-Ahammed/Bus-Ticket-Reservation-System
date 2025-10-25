using FluentValidation;

namespace BusTicket.Application.Features.Payments.Commands.RefundPayment;

public class RefundPaymentCommandValidator : AbstractValidator<RefundPaymentCommand>
{
    public RefundPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty().WithMessage("Payment ID is required");

        RuleFor(x => x.RefundAmount)
            .GreaterThan(0).WithMessage("Refund amount must be greater than zero");

        RuleFor(x => x.RefundReason)
            .NotEmpty().WithMessage("Refund reason is required")
            .MaximumLength(500).WithMessage("Refund reason cannot exceed 500 characters");
    }
}

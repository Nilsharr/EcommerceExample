using Ecommerce.Dtos;
using FluentValidation;

namespace Ecommerce.Validators;

public class PaymentDetailValidator : AbstractValidator<PaymentDetailDto>
{
    public PaymentDetailValidator()
    {
        RuleFor(x => x.Tax).GreaterThanOrEqualTo(0).WithMessage("Tax must be greater than or equal to zero.");

        RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Payment method must not be empty.");

        RuleFor(x => x.Status).NotEmpty().WithMessage("Status must not be empty.");
    }
}
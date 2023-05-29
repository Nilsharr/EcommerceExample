using Ecommerce.Dtos;
using FluentValidation;

namespace Ecommerce.Validators;

public class OrderValidator : AbstractValidator<OrderDto>
{
    public OrderValidator()
    {
        RuleForEach(x => x.Items).NotEmpty().WithMessage("Products are required.")
            .SetValidator(new ProductItemValidator());
        RuleFor(x => x.Status).IsInEnum().WithMessage("Incorrect order status.");
        RuleFor(x => x.PaymentDetails).NotNull().WithMessage("Payment details are required.")
            .SetValidator(new PaymentDetailValidator());
        RuleFor(x => x.ShippingAddress).NotNull().WithMessage("Shipping address is required.")
            .SetValidator(new AddressValidator());
        RuleFor(x => x.ShippingMethod).NotNull().WithMessage("Shipping method is required.")
            .SetValidator(new ShippingMethodValidator());
    }
}
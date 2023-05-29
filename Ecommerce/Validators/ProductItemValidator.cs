using Ecommerce.Dtos;
using FluentValidation;

namespace Ecommerce.Validators;

public class ProductItemValidator : AbstractValidator<ProductItemDto>
{
    public ProductItemValidator()
    {
        RuleFor(x => x.Product).NotNull().WithMessage("Product is required.").SetValidator(new ProductValidator());

        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
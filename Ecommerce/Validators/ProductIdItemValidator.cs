using Ecommerce.Dtos;
using FluentValidation;

namespace Ecommerce.Validators;

public class ProductIdItemValidator : AbstractValidator<ProductIdItemDto>
{
    public ProductIdItemValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product id is required.");

        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
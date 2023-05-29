using Ecommerce.Dtos;
using FluentValidation;

namespace Ecommerce.Validators;

public class ProductValidator : AbstractValidator<ProductDto>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(512).WithMessage("Name must not exceed 512 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1024).WithMessage("Description must not exceed 1024 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.AmountInStock)
            .GreaterThanOrEqualTo(0).WithMessage("Amount in stock must be greater than or equal to 0.");

        RuleFor(x => x.Categories)
            .NotNull().WithMessage("Categories cannot be null.")
            .Must(x => x.Count > 0).WithMessage("At least one category is required.");
    }
}
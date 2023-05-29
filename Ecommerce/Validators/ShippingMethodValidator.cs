﻿using Ecommerce.Dtos;
using FluentValidation;

namespace Ecommerce.Validators;

public class ShippingMethodValidator : AbstractValidator<ShippingMethodDto>
{
    public ShippingMethodValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0.");
    }
}
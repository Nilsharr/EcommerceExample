using Ecommerce.Dtos;
using FluentValidation;

namespace Ecommerce.Validators;

public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
{
    public UserLoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required!")
            .EmailAddress().WithMessage("Invalid email address.")
            .MaximumLength(512).WithMessage("Login cannot have more than 512 characters");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required!")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
        ;
    }
}
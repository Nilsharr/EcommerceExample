using Ecommerce.Dtos;
using Ecommerce.Interfaces;
using Ecommerce.Validators.CustomValidationRules;
using FluentValidation;

namespace Ecommerce.Validators;

public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
{
    public UserRegisterRequestValidator(IRepositoryFactory repositoryFactory)
    {
        var userRepository = repositoryFactory.CreateUserRepository();

        RuleFor(u => u.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(u => u.Surname).NotEmpty().WithMessage("Surname is required.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required!")
            .EmailAddress().WithMessage("Invalid email address.")
            .MaximumLength(512).WithMessage("Login cannot have more than 512 characters")
            .UniqueEmailAddress(userRepository).WithMessage("Email already in use.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required!")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Equal(x => x.ConfirmPassword).WithMessage("Passwords do not match!");
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("You must repeat password")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
    }
}
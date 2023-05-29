using Ecommerce.Interfaces;
using FluentValidation;

namespace Ecommerce.Validators.CustomValidationRules;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> UniqueEmailAddress<T>(
        this IRuleBuilder<T, string> ruleBuilder, IUserRepository userRepository)
    {
        return ruleBuilder.MustAsync(async (email, _) => !await userRepository.Exists(email));
    }
}
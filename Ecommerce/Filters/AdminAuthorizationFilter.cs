using Ecommerce.Enums;
using Ecommerce.Interfaces;
using Ecommerce.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce.Filters;

public class AdminAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly IUserRepository _userRepository;

    public AdminAuthorizationFilter(IRepositoryFactory repositoryFactory)
    {
        _userRepository = repositoryFactory.CreateUserRepository();
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.GetUserIdFromClaim();
        if (userId is null || await _userRepository.GetRole(userId) != UserRole.Admin)
        {
            context.Result = new ForbidResult();
        }
    }
}
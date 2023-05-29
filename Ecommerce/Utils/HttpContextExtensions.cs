using System.Security.Claims;

namespace Ecommerce.Utils;

public static class HttpContextExtensions
{
    public static string? GetUserIdFromClaim(this HttpContext context) =>
        context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
}
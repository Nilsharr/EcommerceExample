using System.Text.Json.Serialization;

namespace Ecommerce.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    User,
    Admin
}

public static class UserRoleExtensions
{
    public static string GetName(this UserRole userRole) =>
        userRole switch
        {
            UserRole.User => "User",
            UserRole.Admin => "Admin",
            _ => throw new ArgumentOutOfRangeException(nameof(userRole))
        };
}
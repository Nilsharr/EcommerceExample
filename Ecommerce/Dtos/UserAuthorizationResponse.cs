using System.Text.Json.Serialization;
using Ecommerce.Enums;

namespace Ecommerce.Dtos;

public class UserAuthorizationResponse
{
    public string AuthToken { get; set; } = default!;
    public DateTime TokenExpireDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }
}
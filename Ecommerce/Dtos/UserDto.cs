using System.Text.Json.Serialization;
using Ecommerce.Enums;

namespace Ecommerce.Dtos;

public class UserDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string Email { get; set; } = default!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }

    public IList<AddressDto> Addresses { get; set; } = new List<AddressDto>();

    public IList<ProductItemDto> ShoppingCart { get; set; } = new List<ProductItemDto>();
    [JsonIgnore] public string Password { get; set; } = default!;
    [JsonIgnore] public string Salt { get; set; } = default!;
}
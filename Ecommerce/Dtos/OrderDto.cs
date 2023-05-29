using System.Text.Json.Serialization;
using Ecommerce.Enums;

namespace Ecommerce.Dtos;

public class OrderDto
{
    public string? Id { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus? Status { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public PaymentDetailDto PaymentDetails { get; set; } = default!;
    public AddressDto ShippingAddress { get; set; } = default!;
    public ShippingMethodDto ShippingMethod { get; set; } = default!;
    public IList<ProductItemDto> Items { get; set; } = new List<ProductItemDto>();
    [JsonIgnore] public string? UserId { get; set; }
}
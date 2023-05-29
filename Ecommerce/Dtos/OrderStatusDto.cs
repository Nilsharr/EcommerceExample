using System.Text.Json.Serialization;
using Ecommerce.Enums;

namespace Ecommerce.Dtos;

public class OrderStatusDto
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus Status { get; set; }
}
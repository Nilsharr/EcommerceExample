using System.Text.Json.Serialization;

namespace Ecommerce.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Completed,
    Cancelled,
    Returned,
    Refunded,
    Error
}

public static class OrderStatusExtensions
{
    public static string GetName(this OrderStatus orderStatus) =>
        orderStatus switch
        {
            OrderStatus.Pending => "Pending",
            OrderStatus.Processing => "Processing",
            OrderStatus.Shipped => "Shipped",
            OrderStatus.Delivered => "Delivered",
            OrderStatus.Completed => "Completed",
            OrderStatus.Cancelled => "Cancelled",
            OrderStatus.Returned => "Returned",
            OrderStatus.Refunded => "Refunded",
            OrderStatus.Error => "Error",
            _ => throw new ArgumentOutOfRangeException(nameof(orderStatus))
        };
}
using Cassandra.Mapping.Attributes;
using Ecommerce.Entities.Cassandra.Types;

namespace Ecommerce.Entities.Cassandra;

[Table("orders_by_id")]
public class OrdersById
{
    [PartitionKey] [Column("order_id")] public Guid OrderId { get; set; }
    [ClusteringKey] [Column("user_id")] public Guid UserId { get; set; }
    [Column("status")] [StaticColumn] public string Status { get; set; } = default!;

    [Column("product_id")] public Guid ProductId { get; set; }
    [Column("product_name")] public string ProductName { get; set; } = default!;
    [Column("product_description")] public string ProductDescription { get; set; } = default!;
    [Column("product_price")] public decimal ProductPrice { get; set; }
    [Column("product_quantity")] public int ProductQuantity { get; set; }

    [Column("payment_details")]
    [StaticColumn]
    public PaymentDetail PaymentDetails { get; set; } = default!;

    [Column("shipping_address")]
    [StaticColumn]
    public Address ShippingAddress { get; set; } = default!;

    [Column("shipping_method")]
    [StaticColumn]
    public ShippingMethod ShippingMethod { get; set; } = default!;
}
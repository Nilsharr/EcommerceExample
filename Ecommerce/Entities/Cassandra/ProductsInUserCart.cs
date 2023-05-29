using Cassandra.Mapping.Attributes;

namespace Ecommerce.Entities.Cassandra;

[Table("products_in_user_cart")]
public class ProductsInUserCart
{
    [PartitionKey] [Column("user_id")] public Guid UserId { get; set; }
    [ClusteringKey] [Column("product_id")] public Guid ProductId { get; set; }
    [Column("product_name")] public string Name { get; set; } = default!;
    [Column("product_description")] public string Description { get; set; } = default!;
    [Column("product_price")] public decimal Price { get; set; }
    [Column("quantity")] public int Quantity { get; set; }
    [Column("image")] public byte[] Image { get; set; } = default!;
}
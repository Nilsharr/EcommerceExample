using Cassandra.Mapping.Attributes;

namespace Ecommerce.Entities.Cassandra;

[Table("products_by_name")]
public class ProductsByName
{
    [ClusteringKey] [Column("product_id")] public Guid ProductId { get; set; }
    [PartitionKey] [Column("name")] public string Name { get; set; } = default!;
    [Column("description")] public string Description { get; set; } = default!;
    [Column("price")] public decimal Price { get; set; }
    [Column("amount_in_stock")] public int AmountInStock { get; set; }
    [Column("categories")] public IList<string> Categories { get; set; } = new List<string>();
    [Column("image")] public byte[] Image { get; set; } = default!;
}
using Cassandra.Mapping.Attributes;

namespace Ecommerce.Entities.Cassandra;

[Table("products_by_id")]
public class ProductsById
{
    [PartitionKey] [Column("product_id")] public Guid ProductId { get; set; }
    [Column("name")] public string Name { get; set; } = default!;
    [Column("description")] public string Description { get; set; } = default!;
    [Column("price")] public decimal Price { get; set; }
    [Column("amount_in_stock")] public int AmountInStock { get; set; }
    [Column("categories")] public IList<string> Categories { get; set; } = new List<string>();
    [Column("image")] public byte[] Image { get; set; } = default!;
}
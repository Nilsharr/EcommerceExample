namespace Ecommerce.Entities.Mongo.Types;

public class ProductItem
{
    public Product Product { get; set; } = default!;
    public int Quantity { get; set; }
}
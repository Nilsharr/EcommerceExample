namespace Ecommerce.Entities.Cassandra.Types;

public class ShippingMethod
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
}
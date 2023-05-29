namespace Ecommerce.Entities.Mongo.Types;

public class ShippingMethod
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
}
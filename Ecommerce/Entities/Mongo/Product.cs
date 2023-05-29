using MongoDB.Bson;

namespace Ecommerce.Entities.Mongo;

public class Product
{
    public ObjectId Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public int AmountInStock { get; set; }
    public IList<string> Categories { get; set; } = new List<string>();
    public byte[] Image { get; set; } = default!;
}
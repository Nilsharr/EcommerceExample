using Ecommerce.Entities.Mongo.Types;
using Ecommerce.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ecommerce.Entities.Mongo;

public class Order
{
    public ObjectId Id { get; set; }
    public ObjectId UserId { get; set; }
    public IList<ProductItem> Items { get; set; } = new List<ProductItem>();
    [BsonRepresentation(BsonType.String)] public OrderStatus Status { get; set; }
    public PaymentDetail PaymentDetails { get; set; } = default!;
    public Address ShippingAddress { get; set; } = default!;
    public ShippingMethod ShippingMethod { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
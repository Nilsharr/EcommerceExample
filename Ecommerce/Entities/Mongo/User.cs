using Ecommerce.Entities.Mongo.Types;
using Ecommerce.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ecommerce.Entities.Mongo;

public class User
{
    public ObjectId Id { get; set; }
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Salt { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Surname { get; set; } = default!;
    [BsonRepresentation(BsonType.String)] public UserRole Role { get; set; }
    public IList<Address> Addresses { get; set; } = new List<Address>();
    public IList<ProductItem> ShoppingCart { get; set; } = new List<ProductItem>();
    public IList<ObjectId> OrderIds { get; set; } = new List<ObjectId>();
}
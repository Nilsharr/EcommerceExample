using MongoDB.Bson;

namespace Ecommerce.Entities.Mongo.Types;

public class Address
{
    public ObjectId Id { get; set; }
    public string Street { get; set; } = default!;
    public string BuildingNumber { get; set; } = default!;
    public string? HouseNumber { get; set; }
    public string PostalCode { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
}
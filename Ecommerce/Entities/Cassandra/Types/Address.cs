namespace Ecommerce.Entities.Cassandra.Types;

public class Address
{
    public Guid Id { get; set; }
    public string Street { get; set; } = default!;
    public string BuildingNumber { get; set; } = default!;
    public string? HouseNumber { get; set; }
    public string PostalCode { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
}
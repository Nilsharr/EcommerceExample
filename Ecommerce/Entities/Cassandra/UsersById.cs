using Cassandra.Mapping.Attributes;
using Ecommerce.Entities.Cassandra.Types;
using Ecommerce.Enums;

namespace Ecommerce.Entities.Cassandra;

[Table("users_by_id")]
public class UsersById
{
    [PartitionKey] [Column("user_id")] public Guid UserId { get; set; }
    [Column("email")] public string Email { get; set; } = default!;
    [Column("password")] public string Password { get; set; } = default!;
    [Column("salt")] public string Salt { get; set; } = default!;
    [Column("role")] public string Role { get; set; } = default!;
    [Column("name")] public string Name { get; set; } = default!;
    [Column("surname")] public string Surname { get; set; } = default!;
    [Column("addresses")] public IList<Address> Addresses { get; set; } = new List<Address>();
}
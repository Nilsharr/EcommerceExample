using Cassandra.Mapping.Attributes;
using Ecommerce.Enums;

namespace Ecommerce.Entities.Cassandra;

[Table("users_by_email")]
public class UsersByEmail
{
    [Column("user_id")] public Guid UserId { get; set; }
    [PartitionKey] [Column("email")] public string Email { get; set; } = default!;
    [Column("password")] public string Password { get; set; } = default!;
    [Column("salt")] public string Salt { get; set; } = default!;
    [Column("role")] public string Role { get; set; } = default!;
}
using Cassandra.Mapping;
using Cassandra.Mapping.Attributes;

namespace Ecommerce.Entities.Cassandra;

[Table("orders_by_user")]
public class OrdersByUser
{
    [ClusteringKey(0, SortOrder.Ascending)]
    [Column("order_id")]
    public Guid OrderId { get; set; }

    [PartitionKey] [Column("user_id")] public Guid UserId { get; set; }

    [ClusteringKey(2, SortOrder.Ascending)]
    [Column("status")]
    public string Status { get; set; } = default!;

    [ClusteringKey(1, SortOrder.Descending)]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")] public DateTime UpdatedAt { get; set; }
}
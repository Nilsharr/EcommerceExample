using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class OrderStatus : IEntityTypeConfiguration<OrderStatus>
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public virtual IList<Order> Orders { get; set; } = new List<Order>();

    void IEntityTypeConfiguration<OrderStatus>.Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(256);
    }
}
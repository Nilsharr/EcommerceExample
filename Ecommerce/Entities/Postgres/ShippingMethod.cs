using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class ShippingMethod : IEntityTypeConfiguration<ShippingMethod>
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public decimal Price { get; set; }

    public virtual IList<Order> Orders { get; set; } = new List<Order>();

    void IEntityTypeConfiguration<ShippingMethod>.Configure(EntityTypeBuilder<ShippingMethod> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(512);
        builder.Property(e => e.Price).IsRequired().HasPrecision(8, 2);
    }
}
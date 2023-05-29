using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class ProductCategory : IEntityTypeConfiguration<ProductCategory>
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public virtual IList<Product> Products { get; set; } = new List<Product>();

    void IEntityTypeConfiguration<ProductCategory>.Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(512);
    }
}
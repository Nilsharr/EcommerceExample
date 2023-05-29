using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class Product : IEntityTypeConfiguration<Product>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public decimal Price { get; set; }

    public int AmountInStock { get; set; }

    public byte[]? Image { get; set; }

    public virtual IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual IList<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();

    public virtual IList<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    void IEntityTypeConfiguration<Product>.Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.Name).IsRequired().HasMaxLength(512);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(1024);
        builder.Property(e => e.Price).IsRequired().HasPrecision(10, 2);
        builder.Property(e => e.AmountInStock).IsRequired();
        builder.Property(e => e.Image);

        builder.HasMany(d => d.ProductCategories).WithMany(p => p.Products)
            .UsingEntity<Dictionary<string, object>>(
                "ProductCategoriesMembership",
                r => r.HasOne<ProductCategory>().WithMany()
                    .HasForeignKey("ProductCategoryId")
                    .HasConstraintName("product_categories_membership_product_category_id_fkey"),
                l => l.HasOne<Product>().WithMany()
                    .HasForeignKey("ProductId")
                    .HasConstraintName("product_categories_membership_product_id_fkey"),
                j =>
                {
                    j.HasKey("ProductId", "ProductCategoryId").HasName("product_categories_membership_pkey");
                    j.ToTable("product_categories_membership");
                    j.IndexerProperty<Guid>("ProductId").HasColumnName("product_id");
                    j.IndexerProperty<int>("ProductCategoryId").HasColumnName("product_category_id");
                });
    }
}
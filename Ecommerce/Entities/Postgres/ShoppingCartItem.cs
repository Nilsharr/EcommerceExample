using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class ShoppingCartItem : IEntityTypeConfiguration<ShoppingCartItem>
{
    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public User User { get; set; } = default!;

    public Product Product { get; set; } = default!;

    void IEntityTypeConfiguration<ShoppingCartItem>.Configure(EntityTypeBuilder<ShoppingCartItem> builder)
    {
        builder.HasKey(e => new {e.UserId, e.ProductId});
        builder.Property(e => e.Quantity).IsRequired();

        builder.HasOne(d => d.Product).WithMany(p => p.ShoppingCartItems)
            .HasForeignKey(d => d.ProductId);

        builder.HasOne(d => d.User).WithMany(p => p.ShoppingCartItems)
            .HasForeignKey(d => d.UserId);
    }
}
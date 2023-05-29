using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class Order : IEntityTypeConfiguration<Order>
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int OrderStatusId { get; set; }

    public int PaymentDetailId { get; set; }

    public Guid ShippingAddressId { get; set; }

    public int ShippingMethodId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public OrderStatus OrderStatus { get; set; } = default!;

    public PaymentDetail PaymentDetails { get; set; } = default!;

    public Address ShippingAddress { get; set; } = default!;

    public ShippingMethod ShippingMethod { get; set; } = default!;

    public User User { get; set; } = default!;
    public virtual IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    void IEntityTypeConfiguration<Order>.Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.OrderStatusId).IsRequired();
        builder.Property(e => e.PaymentDetailId).IsRequired();
        builder.Property(e => e.ShippingAddressId).IsRequired();
        builder.Property(e => e.ShippingMethodId).IsRequired();
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnType("timestamp without time zone");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnType("timestamp without time zone");

        builder.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
            .HasForeignKey(d => d.OrderStatusId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(d => d.PaymentDetails).WithMany(p => p.Orders)
            .HasForeignKey(d => d.PaymentDetailId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(d => d.ShippingAddress).WithMany(p => p.Orders)
            .HasForeignKey(d => d.ShippingAddressId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(d => d.ShippingMethod).WithMany(p => p.Orders)
            .HasForeignKey(d => d.ShippingMethodId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(d => d.User).WithMany(p => p.Orders)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
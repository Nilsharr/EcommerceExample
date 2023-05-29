using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class PaymentDetail : IEntityTypeConfiguration<PaymentDetail>
{
    public int Id { get; set; }

    public decimal NettoPrice { get; set; }

    public decimal BruttoPrice { get; set; }

    public decimal Tax { get; set; }

    public int PaymentMethodId { get; set; }

    public int PaymentStatusId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = default!;

    public PaymentStatus PaymentStatus { get; set; } = default!;

    public virtual IList<Order> Orders { get; set; } = new List<Order>();


    void IEntityTypeConfiguration<PaymentDetail>.Configure(EntityTypeBuilder<PaymentDetail> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.NettoPrice).IsRequired().HasPrecision(10, 2);
        builder.Property(e => e.BruttoPrice).IsRequired().HasPrecision(10, 2);
        builder.Property(e => e.Tax).IsRequired().HasPrecision(8, 2);
        builder.Property(e => e.PaymentMethodId).IsRequired();
        builder.Property(e => e.PaymentStatusId).IsRequired();

        builder.HasOne(d => d.PaymentMethod).WithMany(p => p.PaymentDetails)
            .HasForeignKey(d => d.PaymentMethodId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(d => d.PaymentStatus).WithMany(p => p.PaymentDetails)
            .HasForeignKey(d => d.PaymentStatusId)
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}
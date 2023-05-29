using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class PaymentStatus : IEntityTypeConfiguration<PaymentStatus>
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public virtual IList<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();

    void IEntityTypeConfiguration<PaymentStatus>.Configure(EntityTypeBuilder<PaymentStatus> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(512);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class PaymentMethod : IEntityTypeConfiguration<PaymentMethod>
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public virtual IList<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();

    void IEntityTypeConfiguration<PaymentMethod>.Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(512);
    }
}
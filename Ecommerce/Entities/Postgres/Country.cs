using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class Country : IEntityTypeConfiguration<Country>
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;
    public string CountryCode { get; set; } = default!;
    public string Code { get; set; } = default!;

    public virtual IList<Address> Addresses { get; set; } = new List<Address>();

    void IEntityTypeConfiguration<Country>.Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(512);
    }
}
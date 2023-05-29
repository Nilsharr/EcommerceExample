using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class Address : IEntityTypeConfiguration<Address>
{
    public Guid Id { get; set; }

    public string Street { get; set; } = default!;

    public string BuildingNumber { get; set; } = default!;

    public string? HouseNumber { get; set; }

    public string PostalCode { get; set; } = default!;

    public string City { get; set; } = default!;

    public int CountryId { get; set; }

    public Guid UserId { get; set; }

    public Country Country { get; set; } = default!;
    public User User { get; set; } = default!;

    public virtual IList<Order> Orders { get; set; } = new List<Order>();

    void IEntityTypeConfiguration<Address>.Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.Street).IsRequired().HasMaxLength(1024);
        builder.Property(e => e.BuildingNumber).IsRequired().HasMaxLength(512);
        builder.Property(e => e.HouseNumber).HasMaxLength(512);
        builder.Property(e => e.PostalCode).IsRequired().HasMaxLength(256);
        builder.Property(e => e.City).IsRequired().HasMaxLength(1024);
        builder.Property(e => e.CountryId).IsRequired();
        builder.Property(e => e.UserId).IsRequired();

        builder.HasOne(d => d.Country).WithMany(p => p.Addresses)
            .HasForeignKey(d => d.CountryId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(d => d.User).WithMany(p => p.Addresses)
            .HasForeignKey(d => d.UserId);
    }
}
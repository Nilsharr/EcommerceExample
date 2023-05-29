using Ecommerce.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Entities.Postgres;

public class User : IEntityTypeConfiguration<User>
{
    public Guid Id { get; set; }

    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string Salt { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Surname { get; set; } = default!;

    public UserRole Role { get; set; }

    public virtual IList<Order> Orders { get; set; } = new List<Order>();
    public virtual IList<Address> Addresses { get; set; } = new List<Address>();
    public virtual IList<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();

    void IEntityTypeConfiguration<User>.Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id).HasName("user_pkey");
        builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.Email).IsRequired().HasMaxLength(512);
        builder.Property(e => e.Password).IsRequired().HasMaxLength(512);
        builder.Property(e => e.Salt).IsRequired().HasMaxLength(256);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(512);
        builder.Property(e => e.Surname).IsRequired().HasMaxLength(512);
        builder.Property(e => e.Role).HasConversion<string>().IsRequired().HasDefaultValue(UserRole.User);

        builder.HasIndex(e => e.Email, "user_email_key").IsUnique();
    }
}
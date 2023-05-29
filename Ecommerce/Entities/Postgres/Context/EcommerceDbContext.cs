using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Entities.Postgres.Context;

public class EcommerceDbContext : DbContext
{
    public EcommerceDbContext()
    {
    }

    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcommerceDbContext).Assembly);
    }

    public DbSet<Address> Addresses { get; set; } = default!;

    public DbSet<Country> Countries { get; set; } = default!;

    public DbSet<Order> Orders { get; set; } = default!;

    public DbSet<OrderItem> OrderItems { get; set; } = default!;

    public DbSet<OrderStatus> OrderStatuses { get; set; } = default!;

    public DbSet<PaymentDetail> PaymentDetails { get; set; } = default!;

    public DbSet<PaymentMethod> PaymentMethods { get; set; } = default!;

    public DbSet<PaymentStatus> PaymentStatuses { get; set; } = default!;

    public DbSet<Product> Products { get; set; } = default!;

    public DbSet<ProductCategory> ProductCategories { get; set; } = default!;

    public DbSet<ShippingMethod> ShippingMethods { get; set; } = default!;

    public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = default!;

    public DbSet<User> Users { get; set; } = default!;
}
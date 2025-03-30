using Crayon.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Crayon.Repository;

public class CrayonDbContext(DbContextOptions<CrayonDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("crayon");
        modelBuilder.Entity<User>().ToTable("user");
        modelBuilder.Entity<Customer>().ToTable("customer");
        modelBuilder.Entity<Account>().ToTable("account");
        modelBuilder.Entity<Order>().ToTable("order");
        modelBuilder.Entity<OrderItem>().ToTable("order_item");
        modelBuilder.Entity<Subscription>().ToTable("subscription");
        modelBuilder.Entity<Licence>().ToTable("licence");
        
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
     
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Licence> Licences { get; set; }
}
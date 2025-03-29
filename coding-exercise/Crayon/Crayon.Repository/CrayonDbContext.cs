using Crayon.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Crayon.Repository;

public class CrayonDbContext : DbContext
{
    public CrayonDbContext(DbContextOptions<CrayonDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
            .ToTable("user", "crayon");
        modelBuilder.Entity<Customer>()
            .ToTable("customer", "crayon");
        modelBuilder.Entity<Account>()
            .ToTable("account", "crayon");
        modelBuilder.Entity<Order>()
            .ToTable("order", "crayon");
        modelBuilder.Entity<OrderItem>()
            .ToTable("order_item", "crayon");
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}
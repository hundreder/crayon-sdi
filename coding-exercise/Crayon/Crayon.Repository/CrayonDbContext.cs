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
        modelBuilder.Entity<User>()
            .ToTable("user", "crayon");
        modelBuilder.Entity<Customer>()
            .ToTable("customer", "crayon");
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
}
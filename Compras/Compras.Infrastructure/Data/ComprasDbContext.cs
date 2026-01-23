using Compras.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Compras.Infrastructure.Data;

public sealed class ComprasDbContext : DbContext
{
    public ComprasDbContext(DbContextOptions<ComprasDbContext> options)
        : base(options)
    {
    }

    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ComprasDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

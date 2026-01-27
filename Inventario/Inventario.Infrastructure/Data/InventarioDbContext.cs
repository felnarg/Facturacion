using Inventario.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Infrastructure.Data;

public sealed class InventarioDbContext : DbContext
{
    public InventarioDbContext(DbContextOptions<InventarioDbContext> options)
        : base(options)
    {
    }

    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventarioDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

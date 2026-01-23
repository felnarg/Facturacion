using Microsoft.EntityFrameworkCore;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Data;

public sealed class VentasDbContext : DbContext
{
    public VentasDbContext(DbContextOptions<VentasDbContext> options)
        : base(options)
    {
    }

    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VentasDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

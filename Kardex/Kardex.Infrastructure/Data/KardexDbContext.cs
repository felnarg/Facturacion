using Kardex.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kardex.Infrastructure.Data;

public sealed class KardexDbContext : DbContext
{
    public KardexDbContext(DbContextOptions<KardexDbContext> options)
        : base(options)
    {
    }

    public DbSet<CreditAccount> CreditAccounts => Set<CreditAccount>();
    public DbSet<CreditMovement> CreditMovements => Set<CreditMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KardexDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

using Inventario.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventario.Infrastructure.Data.Configurations;

public sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");

        builder.HasKey(stockMovement => stockMovement.Id);

        builder.Property(stockMovement => stockMovement.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(stockMovement => stockMovement.CreatedAt)
            .IsRequired();
            
        builder.HasIndex(stockMovement => stockMovement.ProductId);
        builder.HasIndex(stockMovement => stockMovement.ReferenceId);

        builder.HasData(Inventario.Infrastructure.Data.Seed.SeedStocks.GetMovements());
    }
}

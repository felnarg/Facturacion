using Inventario.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventario.Infrastructure.Data.Configurations;

public sealed class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("Stocks");

        builder.HasKey(stock => stock.Id);

        builder.Property(stock => stock.ProductId)
            .IsRequired();

        builder.HasIndex(stock => stock.ProductId)
            .IsUnique();

        builder.Property(stock => stock.Quantity)
            .IsRequired();

        builder.Property(stock => stock.CreatedAt)
            .IsRequired();

        builder.Property(stock => stock.UpdatedAt)
            .IsRequired();

        builder.HasData(Inventario.Infrastructure.Data.Seed.SeedStocks.GetStocks());
    }
}

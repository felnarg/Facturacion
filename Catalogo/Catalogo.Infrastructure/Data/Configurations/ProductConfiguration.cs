using Catalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalogo.Infrastructure.Data.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.Description)
            .HasMaxLength(1000);

        builder.Property(product => product.Sku)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(product => product.SupplierProductCode)
            .IsRequired();

        builder.Property(product => product.InternalProductCode)
            .IsRequired();

        builder.Property(product => product.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(product => product.Stock)
            .IsRequired();

        builder.Property(product => product.CreatedAt)
            .IsRequired();

        builder.Property(product => product.UpdatedAt)
            .IsRequired();

        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new
            {
                Id = Guid.Parse("2b6d7b3f-7c3c-4f6d-a9f5-7c5b5b9e0f01"),
                Name = "Producto base",
                Description = "Producto de ejemplo para el catalogo",
                Price = 19.99m,
                Stock = 100,
                Sku = "SKU-BASE-001",
                SupplierProductCode = 11001,
                InternalProductCode = 50001,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new
            {
                Id = Guid.Parse("9a2e8f9a-6d6d-4b6a-9c5f-1f8b6f3e4e02"),
                Name = "Producto premium",
                Description = "Producto premium de ejemplo",
                Price = 79.99m,
                Stock = 50,
                Sku = "SKU-PREM-002",
                SupplierProductCode = 11002,
                InternalProductCode = 50002,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            });
    }
}

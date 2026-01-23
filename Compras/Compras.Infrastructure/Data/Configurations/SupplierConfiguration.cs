using Compras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compras.Infrastructure.Data.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(supplier => supplier.Id);

        builder.Property(supplier => supplier.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(supplier => supplier.ContactName)
            .HasMaxLength(200);

        builder.Property(supplier => supplier.Phone)
            .HasMaxLength(50);

        builder.Property(supplier => supplier.Email)
            .HasMaxLength(200);

        builder.Property(supplier => supplier.Address)
            .HasMaxLength(300);

        builder.Property(supplier => supplier.CreatedAt)
            .IsRequired();

        builder.HasData(Compras.Infrastructure.Data.Seed.SeedSuppliers.Get());
    }
}

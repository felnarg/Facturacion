using Compras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compras.Infrastructure.Data.Configurations;

public sealed class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");

        builder.HasKey(purchase => purchase.Id);

        builder.Property(purchase => purchase.ProductId)
            .IsRequired();

        builder.Property(purchase => purchase.SupplierId)
            .IsRequired();

        builder.Property(purchase => purchase.SupplierName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(purchase => purchase.Quantity)
            .IsRequired();

        builder.Property(purchase => purchase.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(purchase => purchase.CreatedAt)
            .IsRequired();

        builder.Property(purchase => purchase.ReceivedAt);
    }
}

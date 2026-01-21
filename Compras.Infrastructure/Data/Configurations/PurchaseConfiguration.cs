using Compras.Domain.Entities;
using Compras.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Compras.Infrastructure.Data.Configurations;

public sealed class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");
        builder.HasKey(purchase => purchase.Id);

        builder.Property(purchase => purchase.CustomerId)
            .IsRequired();

        builder.Property(purchase => purchase.PurchasedAt)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(Purchase.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(purchase => purchase.Items, owned =>
        {
            owned.ToTable("PurchaseItems");
            owned.WithOwner().HasForeignKey("PurchaseId");

            owned.Property<Guid>("Id");
            owned.HasKey("Id");

            owned.Property(item => item.ProductId)
                .IsRequired();

            owned.Property(item => item.Name)
                .HasMaxLength(200)
                .IsRequired();

            owned.Property(item => item.Quantity)
                .IsRequired();

            owned.OwnsOne(item => item.UnitPrice, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("UnitPriceAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                price.Property(p => p.Currency)
                    .HasColumnName("UnitPriceCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });
    }
}

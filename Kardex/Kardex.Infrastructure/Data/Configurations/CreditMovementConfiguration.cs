using Kardex.Domain.Entities;
using Kardex.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kardex.Infrastructure.Data.Configurations;

public sealed class CreditMovementConfiguration : IEntityTypeConfiguration<CreditMovement>
{
    public void Configure(EntityTypeBuilder<CreditMovement> builder)
    {
        builder.ToTable("CreditMovements");

        builder.HasKey(movement => movement.Id);

        builder.Property(movement => movement.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(movement => movement.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(movement => movement.DueDate)
            .IsRequired();

        builder.Property(movement => movement.CreatedAt)
            .IsRequired();
    }
}

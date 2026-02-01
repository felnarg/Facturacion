using Kardex.Domain.Entities;
using Kardex.Domain.Enums;
using Kardex.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kardex.Infrastructure.Data.Configurations;

public sealed class CreditAccountConfiguration : IEntityTypeConfiguration<CreditAccount>
{
    public void Configure(EntityTypeBuilder<CreditAccount> builder)
    {
        builder.ToTable("CreditAccounts");

        builder.HasKey(account => account.Id);

        builder.Property(account => account.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(account => account.IdentificationType)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(account => account.IdentificationNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(account => new { account.IdentificationType, account.IdentificationNumber })
            .IsUnique();

        builder.Property(account => account.CreditLimit)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(account => account.PaymentTermDays)
            .IsRequired();

        builder.Property(account => account.CurrentBalance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(account => account.CreatedAt)
            .IsRequired();

        builder.Property(account => account.UpdatedAt)
            .IsRequired();

        builder.HasMany(account => account.Movements)
            .WithOne()
            .HasForeignKey(movement => movement.CreditAccountId);

        builder.HasData(SeedCreditAccounts.Get());
    }
}

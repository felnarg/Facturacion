using Clientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clientes.Infrastructure.Data.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(customer => customer.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(customer => customer.Email)
            .IsUnique();

        builder.Property(customer => customer.City)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(customer => customer.Phone)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(customer => customer.Address)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(customer => customer.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(customer => customer.IdentificationType)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(customer => customer.IdentificationNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(customer => customer.IsCreditApproved)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(customer => customer.ApprovedCreditLimit)
            .HasColumnType("decimal(18,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(customer => customer.ApprovedPaymentTermDays)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(customer => customer.IdentificationNumber)
            .IsUnique();

        builder.Property(customer => customer.Points)
            .IsRequired();

        builder.Property(customer => customer.CreatedAt)
            .IsRequired();

        builder.Property(customer => customer.UpdatedAt)
            .IsRequired();
    }
}

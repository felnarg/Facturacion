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

        builder.Property(customer => customer.Points)
            .IsRequired();

        builder.Property(customer => customer.CreatedAt)
            .IsRequired();

        builder.Property(customer => customer.UpdatedAt)
            .IsRequired();
    }
}

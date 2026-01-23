using Clientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clientes.Infrastructure.Data.Configurations;

public sealed class SaleHistoryConfiguration : IEntityTypeConfiguration<SaleHistory>
{
    public void Configure(EntityTypeBuilder<SaleHistory> builder)
    {
        builder.ToTable("SaleHistories");

        builder.HasKey(history => history.Id);

        builder.Property(history => history.SaleId)
            .IsRequired();

        builder.Property(history => history.ItemsCount)
            .IsRequired();

        builder.Property(history => history.OccurredAt)
            .IsRequired();
    }
}

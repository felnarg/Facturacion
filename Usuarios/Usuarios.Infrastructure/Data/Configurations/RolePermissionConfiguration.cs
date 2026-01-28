using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Usuarios.Domain.Entities;

namespace Usuarios.Infrastructure.Data.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(rp => rp.Id);

        builder.Property(rp => rp.RoleId)
            .IsRequired();

        builder.Property(rp => rp.PermissionId)
            .IsRequired();

        builder.Property(rp => rp.AssignedAt)
            .IsRequired();

        // Índice único compuesto para evitar duplicados
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique();

        // Índices para consultas frecuentes
        builder.HasIndex(rp => rp.RoleId);
        builder.HasIndex(rp => rp.PermissionId);
    }
}

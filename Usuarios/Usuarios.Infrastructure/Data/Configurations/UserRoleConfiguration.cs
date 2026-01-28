using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Usuarios.Domain.Entities;

namespace Usuarios.Infrastructure.Data.Configurations;

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(ur => ur.Id);

        builder.Property(ur => ur.UserId)
            .IsRequired();

        builder.Property(ur => ur.RoleId)
            .IsRequired();

        builder.Property(ur => ur.AssignedAt)
            .IsRequired();

        builder.Property(ur => ur.AssignedBy);

        builder.Property(ur => ur.ExpiresAt);

        // Índice único compuesto para evitar duplicados
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        // Índices para consultas frecuentes
        builder.HasIndex(ur => ur.UserId);
        builder.HasIndex(ur => ur.RoleId);
        builder.HasIndex(ur => ur.ExpiresAt);
    }
}

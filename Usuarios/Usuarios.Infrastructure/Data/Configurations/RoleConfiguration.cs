using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Usuarios.Domain.Entities;

namespace Usuarios.Infrastructure.Data.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(role => role.Code)
            .IsUnique();

        builder.Property(role => role.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(role => role.Description)
            .HasMaxLength(500);

        builder.Property(role => role.HierarchyLevel)
            .IsRequired();

        builder.Property(role => role.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(role => role.IsSystemRole)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(role => role.CreatedAt)
            .IsRequired();

        builder.Property(role => role.UpdatedAt)
            .IsRequired();

        // Relación uno a muchos con UserRoles
        builder.HasMany(role => role.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación uno a muchos con RolePermissions
        builder.HasMany(role => role.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice para búsquedas por estado
        builder.HasIndex(role => role.IsActive);
    }
}

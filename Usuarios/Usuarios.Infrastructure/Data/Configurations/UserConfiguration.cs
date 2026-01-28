using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Usuarios.Domain.Entities;

namespace Usuarios.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(user => user.LastLoginAt);

        builder.Property(user => user.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(user => user.ProfilePictureUrl)
            .HasMaxLength(500);

        builder.Property(user => user.LockedUntil);

        builder.Property(user => user.FailedLoginAttempts)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.UpdatedAt)
            .IsRequired();

        // Relación uno a muchos con UserRoles
        builder.HasMany(user => user.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice para búsquedas por estado
        builder.HasIndex(user => user.IsActive);
    }
}


using Microsoft.EntityFrameworkCore;
using Notificaciones.Domain.Entities;

namespace Notificaciones.Infrastructure.Data;

public class NotificacionesDbContext : DbContext
{
    public DbSet<Notificacion> Notificaciones { get; set; } = null!;

    public NotificacionesDbContext(DbContextOptions<NotificacionesDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.ToTable("Notificaciones");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentoId).IsRequired();
            entity.Property(e => e.NumeroDocumento).HasMaxLength(30).IsRequired();
            entity.Property(e => e.Destinatario).HasMaxLength(256).IsRequired();
            entity.Property(e => e.NombreDestinatario).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Asunto).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Tipo).IsRequired();
            entity.Property(e => e.Canal).IsRequired();
            entity.Property(e => e.Estado).IsRequired();
            entity.Property(e => e.FechaIntento).IsRequired();
            entity.Property(e => e.FechaEnvio).IsRequired(false);
            entity.Property(e => e.ErrorMensaje).HasMaxLength(1000).IsRequired(false);
            entity.Property(e => e.Intentos).IsRequired();

            entity.HasIndex(e => e.DocumentoId);
            entity.HasIndex(e => e.Destinatario);
            entity.HasIndex(e => e.Estado);
        });

        base.OnModelCreating(modelBuilder);
    }
}

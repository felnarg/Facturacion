using FacturacionElectronica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FacturacionElectronica.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Emisor> Emisores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<DocumentoElectronico> DocumentosElectronicos { get; set; }
        public DbSet<NumeracionDocumento> NumeracionesDocumentos { get; set; }
        public DbSet<ItemDocumento> ItemsDocumentos { get; set; }
        public DbSet<ImpuestoDocumento> ImpuestosDocumentos { get; set; }
        public DbSet<PagoDocumento> PagosDocumentos { get; set; }
        public DbSet<EventoDocumento> EventosDocumentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Emisor
            modelBuilder.Entity<Emisor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).HasColumnName("Nit").HasMaxLength(20);
                entity.HasIndex(e => e.Codigo).IsUnique();
                
                entity.Property(e => e.RazonSocial).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NombreComercial).HasMaxLength(200);
                entity.Property(e => e.RegistroMercantil).HasMaxLength(50);
                entity.Property(e => e.ResolucionHabilitacion).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SoftwareId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PinSoftware).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CertificadoDigital).HasColumnType("nvarchar(max)");
                entity.Property(e => e.ClaveCertificado).HasMaxLength(500);

                // Value Objects como propiedades embebidas
                entity.OwnsOne(e => e.Direccion, d =>
                {
                    d.Property(d => d.Calle).IsRequired().HasMaxLength(100);
                    d.Property(d => d.Numero).HasMaxLength(20);
                    d.Property(d => d.Complemento).HasMaxLength(100);
                    d.Property(d => d.Ciudad).IsRequired().HasMaxLength(100);
                    d.Property(d => d.Departamento).IsRequired().HasMaxLength(100);
                    d.Property(d => d.CodigoPostal).HasMaxLength(20);
                    d.Property(d => d.Pais).IsRequired().HasMaxLength(50);
                });

                entity.OwnsOne(e => e.Contacto, c =>
                {
                    c.Property(c => c.Telefono).IsRequired().HasMaxLength(20);
                    c.Property(c => c.Email).IsRequired().HasMaxLength(100);
                    c.Property(c => c.PaginaWeb).HasMaxLength(200);
                });

                entity.HasMany(e => e.Numeraciones)
                      .WithOne(n => n.Emisor)
                      .HasForeignKey(n => n.EmisorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).HasColumnName("Identificacion").HasMaxLength(20);
                entity.HasIndex(e => e.Codigo).IsUnique();
                
                entity.Property(e => e.RazonSocial).IsRequired().HasMaxLength(200);
                entity.Property(e => e.NombreComercial).HasMaxLength(200);
                entity.Property(e => e.RegistroMercantil).HasMaxLength(50);
                entity.Property(e => e.CodigoPostal).HasMaxLength(20);
                entity.Property(e => e.CodigoCiudad).HasMaxLength(10);
                entity.Property(e => e.CodigoDepartamento).HasMaxLength(10);
                entity.Property(e => e.CodigoPais).HasMaxLength(2);

                // Value Objects como propiedades embebidas
                entity.OwnsOne(e => e.Direccion, d =>
                {
                    d.Property(d => d.Calle).IsRequired().HasMaxLength(100);
                    d.Property(d => d.Numero).HasMaxLength(20);
                    d.Property(d => d.Complemento).HasMaxLength(100);
                    d.Property(d => d.Ciudad).IsRequired().HasMaxLength(100);
                    d.Property(d => d.Departamento).IsRequired().HasMaxLength(100);
                    d.Property(d => d.CodigoPostal).HasMaxLength(20);
                    d.Property(d => d.Pais).IsRequired().HasMaxLength(50);
                });

                entity.OwnsOne(e => e.Contacto, c =>
                {
                    c.Property(c => c.Telefono).IsRequired().HasMaxLength(20);
                    c.Property(c => c.Email).IsRequired().HasMaxLength(100);
                    c.Property(c => c.PaginaWeb).HasMaxLength(200);
                });

                entity.HasMany(e => e.Documentos)
                      .WithOne(d => d.Cliente)
                      .HasForeignKey(d => d.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de DocumentoElectronico
            modelBuilder.Entity<DocumentoElectronico>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.NumeroDocumento).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.NumeroDocumento).IsUnique();
                
                entity.Property(e => e.Observaciones).HasMaxLength(500);
                entity.Property(e => e.Cufe).HasMaxLength(100);
                entity.Property(e => e.QrCode).HasColumnType("nvarchar(max)");
                entity.Property(e => e.XmlContent).HasColumnType("nvarchar(max)");
                entity.Property(e => e.XmlFirmado).HasColumnType("nvarchar(max)");
                entity.Property(e => e.RespuestaDian).HasColumnType("nvarchar(max)");
                
                // Value Objects como propiedades embebidas
                entity.OwnsOne(e => e.Subtotal, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });

                entity.OwnsOne(e => e.TotalDescuentos, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });

                entity.OwnsOne(e => e.TotalImpuestos, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });

                entity.OwnsOne(e => e.Total, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });

                entity.OwnsOne(e => e.TotalPagado, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });

                entity.OwnsOne(e => e.SaldoPendiente, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });

                entity.HasOne(e => e.Emisor)
                      .WithMany()
                      .HasForeignKey(e => e.EmisorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Cliente)
                      .WithMany(c => c.Documentos)
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Items)
                      .WithOne(i => i.Documento)
                      .HasForeignKey(i => i.DocumentoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Impuestos)
                      .WithOne(i => i.Documento)
                      .HasForeignKey(i => i.DocumentoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Pagos)
                      .WithOne(p => p.Documento)
                      .HasForeignKey(p => p.DocumentoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Eventos)
                      .WithOne(e => e.Documento)
                      .HasForeignKey(e => e.DocumentoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de NumeracionDocumento
            modelBuilder.Entity<NumeracionDocumento>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Prefijo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.ResolucionAutorizacion).IsRequired().HasMaxLength(100);
                
                entity.HasIndex(e => new { e.EmisorId, e.TipoDocumento, e.Activo })
                      .HasFilter("[Activo] = 1");
            });

            // Configuración de ItemDocumento
            modelBuilder.Entity<ItemDocumento>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CodigoEstandar).HasMaxLength(50);
                entity.Property(e => e.Observaciones).HasMaxLength(500);
                entity.Property(e => e.Cantidad).HasColumnType("decimal(18,3)");
                
                entity.OwnsOne(e => e.ValorUnitario, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });

                entity.OwnsOne(e => e.ValorTotal, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });

                entity.OwnsOne(e => e.Descuento, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });
            });

            // Configuración de ImpuestoDocumento
            modelBuilder.Entity<ImpuestoDocumento>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Porcentaje).HasColumnType("decimal(5,2)");
                entity.Property(e => e.CodigoTributo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.NombreTributo).IsRequired().HasMaxLength(100);
                
                entity.OwnsOne(e => e.BaseGravable, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });

                entity.OwnsOne(e => e.Valor, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });
            });

            // Configuración de PagoDocumento
            modelBuilder.Entity<PagoDocumento>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.MetodoPago).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Referencia).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Observaciones).HasMaxLength(500);
                
                entity.OwnsOne(e => e.Valor, v =>
                {
                    v.Property(v => v.Valor).HasColumnType("decimal(18,2)");
                    v.Property(v => v.Moneda).HasMaxLength(3);
                });
            });

            // Configuración de EventoDocumento
            modelBuilder.Entity<EventoDocumento>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.TipoEvento).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Detalles).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Usuario).IsRequired().HasMaxLength(100);
            });

            // Índices para mejorar el rendimiento
            modelBuilder.Entity<DocumentoElectronico>()
                .HasIndex(e => new { e.EmisorId, e.FechaEmision });

            modelBuilder.Entity<DocumentoElectronico>()
                .HasIndex(e => new { e.ClienteId, e.FechaEmision });

            modelBuilder.Entity<DocumentoElectronico>()
                .HasIndex(e => e.Estado);

            modelBuilder.Entity<DocumentoElectronico>()
                .HasIndex(e => e.FechaEmision);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Actualizar fechas de modificación
            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.ActualizarFechaModificacion();
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
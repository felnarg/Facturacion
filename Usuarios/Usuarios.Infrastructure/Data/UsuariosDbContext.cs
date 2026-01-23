using Microsoft.EntityFrameworkCore;
using Usuarios.Domain.Entities;

namespace Usuarios.Infrastructure.Data;

public sealed class UsuariosDbContext : DbContext
{
    public UsuariosDbContext(DbContextOptions<UsuariosDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsuariosDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

using Microsoft.EntityFrameworkCore;
using Notificaciones.Domain.Entities;
using Notificaciones.Domain.Interfaces;
using Notificaciones.Infrastructure.Data;

namespace Notificaciones.Infrastructure.Repositories;

public sealed class NotificacionRepository : INotificacionRepository
{
    private readonly NotificacionesDbContext _context;

    public NotificacionRepository(NotificacionesDbContext context)
        => _context = context;

    public async Task AddAsync(Notificacion notificacion)
        => await _context.Notificaciones.AddAsync(notificacion);

    public async Task<Notificacion?> GetByIdAsync(Guid id)
        => await _context.Notificaciones.FindAsync(id);

    public async Task<IEnumerable<Notificacion>> GetByDocumentoIdAsync(Guid documentoId)
        => await _context.Notificaciones
            .Where(n => n.DocumentoId == documentoId)
            .OrderByDescending(n => n.FechaIntento)
            .ToListAsync();

    public async Task UpdateAsync(Notificacion notificacion)
    {
        _context.Notificaciones.Update(notificacion);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}

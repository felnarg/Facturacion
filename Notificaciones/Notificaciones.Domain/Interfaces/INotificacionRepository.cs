using Notificaciones.Domain.Entities;

namespace Notificaciones.Domain.Interfaces;

public interface INotificacionRepository
{
    Task AddAsync(Notificacion notificacion);
    Task<Notificacion?> GetByIdAsync(Guid id);
    Task<IEnumerable<Notificacion>> GetByDocumentoIdAsync(Guid documentoId);
    Task UpdateAsync(Notificacion notificacion);
    Task SaveChangesAsync();
}

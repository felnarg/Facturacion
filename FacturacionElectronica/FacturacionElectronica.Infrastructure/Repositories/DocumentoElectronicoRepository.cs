using FacturacionElectronica.Domain.Entities;
using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacturacionElectronica.Infrastructure.Repositories
{
    public class DocumentoElectronicoRepository : Repository<DocumentoElectronico>, IDocumentoElectronicoRepository
    {
        public DocumentoElectronicoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<DocumentoElectronico> GetByNumeroAsync(string numeroDocumento)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.NumeroDocumento == numeroDocumento);
        }

        public async Task<IEnumerable<DocumentoElectronico>> GetByEmisorAsync(string emisorId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var query = _dbSet.Where(d => d.EmisorId == emisorId);

            if (fechaInicio.HasValue)
                query = query.Where(d => d.FechaEmision >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(d => d.FechaEmision <= fechaFin.Value);

            return await query
                .OrderByDescending(d => d.FechaEmision)
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentoElectronico>> GetByClienteAsync(string clienteId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var query = _dbSet.Where(d => d.ClienteId == clienteId);

            if (fechaInicio.HasValue)
                query = query.Where(d => d.FechaEmision >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(d => d.FechaEmision <= fechaFin.Value);

            return await query
                .OrderByDescending(d => d.FechaEmision)
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentoElectronico>> GetByEstadoAsync(EstadoDocumento estado, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var query = _dbSet.Where(d => d.Estado == estado);

            if (fechaInicio.HasValue)
                query = query.Where(d => d.FechaEmision >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(d => d.FechaEmision <= fechaFin.Value);

            return await query
                .OrderByDescending(d => d.FechaEmision)
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentoElectronico>> GetByTipoAsync(TipoDocumento tipo, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var query = _dbSet.Where(d => d.TipoDocumento == tipo);

            if (fechaInicio.HasValue)
                query = query.Where(d => d.FechaEmision >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(d => d.FechaEmision <= fechaFin.Value);

            return await query
                .OrderByDescending(d => d.FechaEmision)
                .ToListAsync();
        }

        public async Task<DocumentoElectronico> GetWithItemsAsync(Guid id)
        {
            return await _dbSet
                .Include(d => d.Items)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DocumentoElectronico> GetWithAllAsync(Guid id)
        {
            return await _dbSet
                .Include(d => d.Items)
                .Include(d => d.Impuestos)
                .Include(d => d.Pagos)
                .Include(d => d.Eventos)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> ExisteNumeroDocumentoAsync(string numeroDocumento)
        {
            return await _dbSet.AnyAsync(d => d.NumeroDocumento == numeroDocumento);
        }

        public async Task<IEnumerable<DocumentoElectronico>> GetPendientesTransmisionAsync()
        {
            return await _dbSet
                .Where(d => d.Estado == EstadoDocumento.Firmado || d.Estado == EstadoDocumento.Validado)
                .Where(d => d.FechaTransmision == null)
                .OrderBy(d => d.FechaEmision)
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentoElectronico>> GetVencidosAsync()
        {
            return await _dbSet
                .Where(d => d.Estado == EstadoDocumento.Aceptado)
                .Where(d => d.FechaVencimiento < DateTime.UtcNow)
                .Where(d => d.SaldoPendiente.Valor > 0)
                .OrderBy(d => d.FechaVencimiento)
                .ToListAsync();
        }
    }
}
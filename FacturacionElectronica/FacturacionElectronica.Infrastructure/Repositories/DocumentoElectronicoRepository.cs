using FacturacionElectronica.Domain.Entities;
using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.Interfaces;
using FacturacionElectronica.Infrastructure.Data;
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
                .AsSplitQuery()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        /// <summary>
        /// Actualiza estado DIAN en BD sin tocar el grafo ya persistido (evita DbUpdateConcurrencyException).
        /// Ejecuta UPDATE del documento e INSERT de los 2 eventos (transmitido + aceptado/rechazado).
        /// </summary>
        public async Task ActualizarEstadoTransmisionDianAsync(Guid documentoId, EstadoDocumento estado, DateTime fechaTransmision, DateTime fechaRespuesta, string respuestaDian, bool aceptado)
        {
            await _context.DocumentosElectronicos
                .Where(d => d.Id == documentoId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.Estado, estado)
                    .SetProperty(d => d.FechaTransmision, fechaTransmision)
                    .SetProperty(d => d.FechaRespuesta, fechaRespuesta)
                    .SetProperty(d => d.RespuestaDian, respuestaDian.Length > 4000 ? respuestaDian.Substring(0, 4000) : respuestaDian)
                    .SetProperty(d => d.FechaModificacion, DateTime.UtcNow));

            var descripcionEvento = aceptado ? "Documento aceptado" : "Documento rechazado";
            var detalleRespuesta = respuestaDian.Length > 200 ? respuestaDian.Substring(0, 200) : respuestaDian;
            var ev1 = new EventoDocumento(documentoId, "Documento transmitido", "Transmitido a la DIAN");
            var ev2 = new EventoDocumento(documentoId, descripcionEvento, detalleRespuesta);
            await _context.EventosDocumentos.AddRangeAsync(ev1, ev2);
        }

        /// <summary>
        /// Recarga el documento y su colección Eventos desde la BD (tras ActualizarEstadoTransmisionDianAsync).
        /// </summary>
        public void ReloadWithEventos(DocumentoElectronico documento)
        {
            _context.Entry(documento).Reload();
            _context.Entry(documento).Collection(d => d.Eventos).Load();
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
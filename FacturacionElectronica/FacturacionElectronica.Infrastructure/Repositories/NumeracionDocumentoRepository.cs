using FacturacionElectronica.Domain.Entities;
using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacturacionElectronica.Infrastructure.Repositories
{
    public class NumeracionDocumentoRepository : Repository<NumeracionDocumento>, INumeracionDocumentoRepository
    {
        public NumeracionDocumentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<NumeracionDocumento> GetByEmisorAndTipoAsync(string emisorId, TipoDocumento tipoDocumento)
        {
            return await _dbSet
                .FirstOrDefaultAsync(n => n.EmisorId == emisorId && n.TipoDocumento == tipoDocumento);
        }

        public async Task<IEnumerable<NumeracionDocumento>> GetByEmisorAsync(string emisorId)
        {
            return await _dbSet
                .Where(n => n.EmisorId == emisorId)
                .ToListAsync();
        }

        public async Task<NumeracionDocumento> GetActivaByEmisorAndTipoAsync(string emisorId, TipoDocumento tipoDocumento)
        {
            var ahora = DateTime.UtcNow;
            return await _dbSet
                .FirstOrDefaultAsync(n => 
                    n.EmisorId == emisorId && 
                    n.TipoDocumento == tipoDocumento && 
                    n.Activo && 
                    n.FechaVencimiento > ahora && 
                    n.NumeroActual <= n.NumeroFinal);
        }

        public async Task<bool> ExisteNumeracionAsync(string prefijo, long numeroInicial, long numeroFinal, string emisorId)
        {
            return await _dbSet.AnyAsync(n => 
                n.Prefijo == prefijo && 
                n.EmisorId == emisorId &&
                ((n.NumeroInicial >= numeroInicial && n.NumeroInicial <= numeroFinal) ||
                 (n.NumeroFinal >= numeroInicial && n.NumeroFinal <= numeroFinal)));
        }
    }
}
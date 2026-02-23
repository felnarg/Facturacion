using FacturacionElectronica.Domain.Entities;
using FacturacionElectronica.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using FacturacionElectronica.Infrastructure.Data;

namespace FacturacionElectronica.Infrastructure.Repositories
{
    public class EmisorRepository : Repository<Emisor>, IEmisorRepository
    {
        public EmisorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Emisor> GetByNitAsync(string nit)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Codigo == nit);
        }

        public async Task<Emisor> GetWithNumeracionesAsync(string nit)
        {
            return await _dbSet
                .Include(e => e.Numeraciones)
                .FirstOrDefaultAsync(e => e.Codigo == nit);
        }

        public async Task<bool> ExisteNitAsync(string nit)
        {
            return await _dbSet.AnyAsync(e => e.Codigo == nit);
        }
    }
}
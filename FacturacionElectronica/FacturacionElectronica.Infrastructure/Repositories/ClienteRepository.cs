using FacturacionElectronica.Domain.Entities;
using FacturacionElectronica.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using FacturacionElectronica.Infrastructure.Data;

namespace FacturacionElectronica.Infrastructure.Repositories
{
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        public ClienteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Cliente> GetByIdentificacionAsync(string identificacion)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Codigo == identificacion);
        }

        public async Task<IEnumerable<Cliente>> SearchByRazonSocialAsync(string razonSocial)
        {
            return await _dbSet
                .Where(c => c.RazonSocial.Contains(razonSocial))
                .ToListAsync();
        }

        public async Task<bool> ExisteIdentificacionAsync(string identificacion)
        {
            return await _dbSet.AnyAsync(c => c.Codigo == identificacion);
        }
    }
}
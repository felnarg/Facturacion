using Microsoft.EntityFrameworkCore;
using Ventas.Domain.Entities;
using Ventas.Domain.Repositories;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Repositories;

public sealed class SaleRepository : ISaleRepository
{
    private readonly VentasDbContext _dbContext;

    public SaleRepository(VentasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sales
            .Include(sale => sale.Items)
            .FirstOrDefaultAsync(sale => sale.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sales
            .Include(sale => sale.Items)
            .OrderByDescending(sale => sale.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _dbContext.Sales.Add(sale);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

using Compras.Domain.Entities;
using Compras.Domain.Repositories;
using Compras.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Compras.Infrastructure.Repositories;

public sealed class PurchaseRepository : IPurchaseRepository
{
    private readonly ComprasDbContext _dbContext;

    public PurchaseRepository(ComprasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Purchases.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Purchase>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Purchases.AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default)
    {
        _dbContext.Purchases.Add(purchase);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Purchase purchase, CancellationToken cancellationToken = default)
    {
        _dbContext.Purchases.Update(purchase);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

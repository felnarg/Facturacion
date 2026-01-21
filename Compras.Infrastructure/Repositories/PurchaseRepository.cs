using Compras.Application.Interfaces;
using Compras.Domain.Entities;
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

    public async Task AddAsync(Purchase purchase, CancellationToken cancellationToken)
    {
        _dbContext.Purchases.Add(purchase);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Purchases
            .Include(purchase => purchase.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(purchase => purchase.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Purchase>> ListAsync(Guid? customerId, CancellationToken cancellationToken)
    {
        var query = _dbContext.Purchases
            .Include(purchase => purchase.Items)
            .AsNoTracking();

        if (customerId.HasValue)
        {
            query = query.Where(purchase => purchase.CustomerId == customerId.Value);
        }

        var purchases = await query
            .OrderByDescending(purchase => purchase.PurchasedAt)
            .ToArrayAsync(cancellationToken);

        return purchases;
    }
}

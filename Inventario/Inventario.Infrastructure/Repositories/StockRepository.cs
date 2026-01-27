using Inventario.Domain.Entities;
using Inventario.Domain.Repositories;
using Inventario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Infrastructure.Repositories;

public sealed class StockRepository : IStockRepository
{
    private readonly InventarioDbContext _dbContext;

    public StockRepository(InventarioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Stock?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Stocks.FirstOrDefaultAsync(stock => stock.ProductId == productId, cancellationToken);
    }

    public async Task<IReadOnlyList<Stock>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Stocks.AsNoTracking()
            .OrderBy(stock => stock.ProductId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Stock stock, CancellationToken cancellationToken = default)
    {
        _dbContext.Stocks.Add(stock);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Stock stock, CancellationToken cancellationToken = default)
    {
        _dbContext.Stocks.Update(stock);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddMovementAsync(StockMovement movement, CancellationToken cancellationToken = default)
    {
        _dbContext.StockMovements.Add(movement);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

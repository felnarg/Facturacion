using Inventario.Domain.Entities;

namespace Inventario.Domain.Repositories;

public interface IStockRepository
{
    Task<Stock?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Stock>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Stock stock, CancellationToken cancellationToken = default);
    Task UpdateAsync(Stock stock, CancellationToken cancellationToken = default);
    Task AddMovementAsync(StockMovement movement, CancellationToken cancellationToken = default);
    
    // Atomic operations that ensure consistency
    Task AddStockWithMovementAsync(Stock stock, StockMovement movement, CancellationToken cancellationToken = default);
    Task UpdateStockWithMovementAsync(Stock stock, StockMovement movement, CancellationToken cancellationToken = default);
}

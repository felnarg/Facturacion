using Inventario.Application.DTOs;

namespace Inventario.Application.Abstractions;

public interface IStockService
{
    Task<StockDto?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<StockDto> CreateForProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<StockDto?> IncreaseAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
    Task<StockDto?> DecreaseAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
    Task<StockDto?> SetAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
}

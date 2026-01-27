using Inventario.Application.Abstractions;
using Inventario.Application.DTOs;
using Inventario.Domain.Entities;
using Inventario.Domain.Repositories;

namespace Inventario.Application.Services;

public sealed class StockService : IStockService
{
    private readonly IStockRepository _repository;

    public StockService(IStockRepository repository)
    {
        _repository = repository;
    }

    public async Task<StockDto?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var stock = await _repository.GetByProductIdAsync(productId, cancellationToken);
        return stock is null ? null : Map(stock);
    }

    public async Task<IReadOnlyList<StockDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var stocks = await _repository.GetAllAsync(cancellationToken);
        return stocks.Select(Map).ToList();
    }

    public async Task<StockDto> CreateForProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByProductIdAsync(productId, cancellationToken);
        if (existing is not null)
        {
            return Map(existing);
        }

        // Create stock and movement - SaveChangesAsync ensures atomicity
        var stock = new Stock(productId);
        var movement = new StockMovement(productId, StockMovementType.Creation, 0, null);
        
        await _repository.AddStockWithMovementAsync(stock, movement, cancellationToken);
        
        return Map(stock);
    }

    public async Task<StockDto?> IncreaseAsync(Guid productId, int quantity, Guid? referenceId, CancellationToken cancellationToken = default)
    {
        var stock = await _repository.GetByProductIdAsync(productId, cancellationToken);
        if (stock is null)
        {
            return null;
        }

        // Update stock and add movement - SaveChangesAsync ensures atomicity
        stock.Increase(quantity);
        var movement = new StockMovement(productId, StockMovementType.Purchase, quantity, referenceId);

        await _repository.UpdateStockWithMovementAsync(stock, movement, cancellationToken);
        return Map(stock);
    }

    public async Task<StockDto?> DecreaseAsync(Guid productId, int quantity, Guid? referenceId, CancellationToken cancellationToken = default)
    {
        var stock = await _repository.GetByProductIdAsync(productId, cancellationToken);
        if (stock is null)
        {
            return null;
        }

        // Update stock and add movement - SaveChangesAsync ensures atomicity
        stock.Decrease(quantity);
        var movement = new StockMovement(productId, StockMovementType.Sale, -quantity, referenceId);
        
        await _repository.UpdateStockWithMovementAsync(stock, movement, cancellationToken);
        return Map(stock);
    }

    public async Task<StockDto?> SetAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var stock = await _repository.GetByProductIdAsync(productId, cancellationToken);
        if (stock is null)
        {
            return null;
        }

        var previousQuantity = stock.Quantity;
        var diff = quantity - previousQuantity;
        
        if (diff == 0)
        {
            return Map(stock);
        }

        // Update stock and add movement - SaveChangesAsync ensures atomicity
        stock.SetQuantity(quantity);
        var movement = new StockMovement(productId, StockMovementType.Adjustment, diff, null);

        await _repository.UpdateStockWithMovementAsync(stock, movement, cancellationToken);
        return Map(stock);
    }

    private static StockDto Map(Stock stock)
    {
        return new StockDto(
            stock.ProductId,
            stock.Quantity,
            stock.CreatedAt,
            stock.UpdatedAt);
    }
}

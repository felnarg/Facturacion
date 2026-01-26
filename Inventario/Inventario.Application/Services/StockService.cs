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

        var stock = new Stock(productId);
        await _repository.AddAsync(stock, cancellationToken);
        return Map(stock);
    }

    public async Task<StockDto?> IncreaseAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var stock = await _repository.GetByProductIdAsync(productId, cancellationToken);
        if (stock is null)
        {
            return null;
        }

        stock.Increase(quantity);
        await _repository.UpdateAsync(stock, cancellationToken);
        return Map(stock);
    }

    public async Task<StockDto?> DecreaseAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var stock = await _repository.GetByProductIdAsync(productId, cancellationToken);
        if (stock is null)
        {
            return null;
        }

        stock.Decrease(quantity);
        await _repository.UpdateAsync(stock, cancellationToken);
        return Map(stock);
    }

    public async Task<StockDto?> SetAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var stock = await _repository.GetByProductIdAsync(productId, cancellationToken);
        if (stock is null)
        {
            return null;
        }

        stock.SetQuantity(quantity);
        await _repository.UpdateAsync(stock, cancellationToken);
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

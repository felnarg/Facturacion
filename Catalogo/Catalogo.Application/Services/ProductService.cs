using Catalogo.Application.Abstractions;
using Catalogo.Application.DTOs;
using Catalogo.Application.Events;
using Catalogo.Domain.Entities;
using Catalogo.Domain.Repositories;

namespace Catalogo.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IEventBus _eventBus;

    public ProductService(IProductRepository repository, IEventBus eventBus)
    {
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : Map(product);
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(cancellationToken);
        return products.Select(Map).ToList();
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = new Product(request.Name, request.Description, request.Price, request.Stock, request.Sku);
        await _repository.AddAsync(product, cancellationToken);

        var createdEvent = new ProductCreatedEvent(
            product.Id,
            product.Name,
            product.Price,
            product.Stock,
            product.Sku,
            DateTime.UtcNow);

        await _eventBus.PublishAsync(createdEvent, "product.created", cancellationToken);

        return Map(product);
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        product.Update(request.Name, request.Description, request.Price, request.Stock, request.Sku);
        await _repository.UpdateAsync(product, cancellationToken);

        var updatedEvent = new ProductUpdatedEvent(
            product.Id,
            product.Name,
            product.Price,
            product.Stock,
            product.Sku,
            DateTime.UtcNow);

        await _eventBus.PublishAsync(updatedEvent, "product.updated", cancellationToken);

        return Map(product);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        await _repository.DeleteAsync(product, cancellationToken);

        var deletedEvent = new ProductDeletedEvent(product.Id, DateTime.UtcNow);
        await _eventBus.PublishAsync(deletedEvent, "product.deleted", cancellationToken);

        return true;
    }

    private static ProductDto Map(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.Sku,
            product.CreatedAt,
            product.UpdatedAt);
    }
}

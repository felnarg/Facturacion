using Compras.Application.Abstractions;
using Compras.Application.DTOs;
using Compras.Domain.Entities;
using Compras.Domain.Repositories;
using Facturacion.Shared.Events;
using Facturacion.Shared.Events;

namespace Compras.Application.Services;

public sealed class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _repository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IEventBus _eventBus;

    public PurchaseService(
        IPurchaseRepository repository,
        ISupplierRepository supplierRepository,
        IEventBus eventBus)
    {
        _repository = repository;
        _supplierRepository = supplierRepository;
        _eventBus = eventBus;
    }

    public async Task<IReadOnlyList<PurchaseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var purchases = await _repository.GetAllAsync(cancellationToken);
        return purchases.Select(Map).ToList();
    }

    public async Task<PurchaseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var purchase = await _repository.GetByIdAsync(id, cancellationToken);
        return purchase is null ? null : Map(purchase);
    }

    public async Task<PurchaseDto> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier is null)
        {
            throw new InvalidOperationException("El proveedor no existe.");
        }

        var purchase = new Purchase(
            request.ProductId,
            request.InternalProductCode,
            request.ProductName,
            request.Quantity,
            supplier.Id,
            supplier.Name,
            request.SupplierInvoiceNumber);
        await _repository.AddAsync(purchase, cancellationToken);

        if (request.OriginalSalePercentage.HasValue &&
            request.SalePercentage != request.OriginalSalePercentage.Value)
        {
            var updatedEvent = new PurchaseSalePercentagesUpdated(
                purchase.Id,
                new[]
                {
                    new SalePercentageUpdatedItem(request.ProductId, request.SalePercentage)
                });
            await _eventBus.PublishAsync(updatedEvent, "product.salepercentage.updated", cancellationToken);
        }
        return Map(purchase);
    }

    public async Task<PurchaseDto?> MarkReceivedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var purchase = await _repository.GetByIdAsync(id, cancellationToken);
        if (purchase is null)
        {
            return null;
        }

        purchase.MarkReceived();
        await _repository.UpdateAsync(purchase, cancellationToken);

        var stockReceived = new StockReceived(purchase.ProductId, purchase.Quantity);
        await _eventBus.PublishAsync(stockReceived, "stock.received", cancellationToken);

        return Map(purchase);
    }

    private static PurchaseDto Map(Purchase purchase)
    {
        return new PurchaseDto(
            purchase.Id,
            purchase.ProductId,
            purchase.InternalProductCode,
            purchase.ProductName,
            purchase.SupplierId,
            purchase.Quantity,
            purchase.SupplierName,
            purchase.SupplierInvoiceNumber,
            purchase.Status,
            purchase.CreatedAt,
            purchase.ReceivedAt);
    }
}

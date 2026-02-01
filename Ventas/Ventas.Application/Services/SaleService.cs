using Facturacion.Shared.Events;
using Ventas.Application.Abstractions;
using Ventas.Application.DTOs;
using Ventas.Domain.Entities;
using Ventas.Domain.Repositories;

namespace Ventas.Application.Services;

public sealed class SaleService : ISaleService
{
    private readonly ISaleRepository _repository;
    private readonly IEventBus _eventBus;

    public SaleService(ISaleRepository repository, IEventBus eventBus)
    {
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<IReadOnlyList<SaleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sales = await _repository.GetAllAsync(cancellationToken);
        return sales.Select(Map).ToList();
    }

    public async Task<SaleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _repository.GetByIdAsync(id, cancellationToken);
        return sale is null ? null : Map(sale);
    }

    public async Task<SaleDto> CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default)
    {
        var items = request.Items.Select(item => new Ventas.Domain.Entities.SaleItem(item.ProductId, item.Quantity)).ToList();
        var sale = new Sale(items);
        await _repository.AddAsync(sale, cancellationToken);

        var saleCompleted = new SaleCompleted(
            sale.Id,
            sale.Items.Select(item => new Facturacion.Shared.Events.SaleItem(item.ProductId, item.Quantity)).ToList());

        await _eventBus.PublishAsync(saleCompleted, "sale.completed", cancellationToken);

        if (string.Equals(request.PaymentMethod, "credito", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(request.CustomerIdentificationType) ||
                string.IsNullOrWhiteSpace(request.CustomerIdentification))
            {
                throw new InvalidOperationException("La venta a crédito requiere CC o NIT.");
            }

            if (!request.TotalAmount.HasValue || request.TotalAmount.Value <= 0)
            {
                throw new InvalidOperationException("La venta a crédito requiere un total válido.");
            }

            var creditSaleRequested = new CreditSaleRequested(
                sale.Id,
                request.CustomerIdentificationType,
                request.CustomerIdentification,
                request.TotalAmount.Value,
                DateTime.UtcNow);

            await _eventBus.PublishAsync(creditSaleRequested, "sale.credit.requested", cancellationToken);
        }

        return Map(sale);
    }

    private static SaleDto Map(Sale sale)
    {
        return new SaleDto(
            sale.Id,
            sale.CreatedAt,
            sale.Items.Select(item => new SaleItemDto(item.ProductId, item.Quantity)).ToList());
    }
}

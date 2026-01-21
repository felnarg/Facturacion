using Compras.Application.Exceptions;
using Compras.Application.Interfaces;
using Compras.Application.Models;
using Compras.Domain.Entities;
using Compras.Domain.ValueObjects;

namespace Compras.Application.Services;

public sealed class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _repository;
    private readonly IDateTimeProvider _clock;

    public PurchaseService(IPurchaseRepository repository, IDateTimeProvider clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task<PurchaseDto> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken)
    {
        var errors = Validate(request);
        if (errors.Any())
        {
            throw new ValidationException(errors);
        }

        var purchase = new Purchase(Guid.NewGuid(), request.CustomerId, _clock.UtcNow);

        foreach (var item in request.Items)
        {
            var unitPrice = new Money(item.UnitPrice, request.Currency);
            purchase.AddItem(new PurchaseItem(item.ProductId, item.Name, unitPrice, item.Quantity));
        }

        _ = purchase.Total();

        await _repository.AddAsync(purchase, cancellationToken);

        return MapToDto(purchase);
    }

    public async Task<PurchaseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var purchase = await _repository.GetByIdAsync(id, cancellationToken);
        if (purchase is null)
        {
            throw new NotFoundException("La compra no existe.");
        }

        return MapToDto(purchase);
    }

    public async Task<IReadOnlyCollection<PurchaseDto>> ListAsync(Guid? customerId, CancellationToken cancellationToken)
    {
        var purchases = await _repository.ListAsync(customerId, cancellationToken);
        return purchases.Select(MapToDto).ToArray();
    }

    private static IReadOnlyCollection<string> Validate(CreatePurchaseRequest request)
    {
        var errors = new List<string>();

        if (request.CustomerId == Guid.Empty)
        {
            errors.Add("El cliente es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            errors.Add("La moneda es obligatoria.");
        }

        if (request.Items.Count == 0)
        {
            errors.Add("Debe registrar al menos un item.");
        }

        foreach (var item in request.Items)
        {
            if (item.ProductId == Guid.Empty)
            {
                errors.Add("El producto es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(item.Name))
            {
                errors.Add("El nombre del producto es obligatorio.");
            }

            if (item.UnitPrice <= 0)
            {
                errors.Add("El precio unitario debe ser mayor que cero.");
            }

            if (item.Quantity <= 0)
            {
                errors.Add("La cantidad debe ser mayor que cero.");
            }
        }

        return errors;
    }

    private static PurchaseDto MapToDto(Purchase purchase)
    {
        var total = purchase.Total();

        return new PurchaseDto
        {
            Id = purchase.Id,
            CustomerId = purchase.CustomerId,
            PurchasedAt = purchase.PurchasedAt,
            Currency = total.Currency,
            Total = total.Amount,
            Items = purchase.Items.Select(item =>
            {
                var itemTotal = item.Total();
                return new PurchaseItemDto
                {
                    ProductId = item.ProductId,
                    Name = item.Name,
                    UnitPrice = item.UnitPrice.Amount,
                    Quantity = item.Quantity,
                    Total = itemTotal.Amount
                };
            }).ToArray()
        };
    }
}

using Compras.Domain.Exceptions;
using Compras.Domain.ValueObjects;

namespace Compras.Domain.Entities;

public sealed class Purchase
{
    private readonly List<PurchaseItem> _items = new();

    public Guid Id { get; }
    public Guid CustomerId { get; }
    public DateTimeOffset PurchasedAt { get; }
    public IReadOnlyCollection<PurchaseItem> Items => _items.AsReadOnly();

    public Purchase(Guid id, Guid customerId, DateTimeOffset purchasedAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("El identificador de la compra es obligatorio.");
        }

        if (customerId == Guid.Empty)
        {
            throw new DomainException("El cliente es obligatorio.");
        }

        Id = id;
        CustomerId = customerId;
        PurchasedAt = purchasedAt;
    }

    public void AddItem(PurchaseItem item)
    {
        if (item is null)
        {
            throw new DomainException("El item es obligatorio.");
        }

        _items.Add(item);
    }

    public Money Total()
    {
        if (!_items.Any())
        {
            throw new DomainException("La compra debe tener al menos un item.");
        }

        var currency = _items.First().UnitPrice.Currency;
        var total = new Money(0, currency);

        foreach (var item in _items)
        {
            total = total.Add(item.Total());
        }

        return total;
    }
}

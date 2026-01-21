using Compras.Domain.Exceptions;
using Compras.Domain.ValueObjects;

namespace Compras.Domain.Entities;

public sealed class PurchaseItem
{
    public Guid ProductId { get; private set; }
    public string Name { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public PurchaseItem(Guid productId, string name, Money unitPrice, int quantity)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("El producto es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre del producto es obligatorio.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("La cantidad debe ser mayor que cero.");
        }

        ProductId = productId;
        Name = name.Trim();
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    private PurchaseItem()
    {
        ProductId = Guid.Empty;
        Name = string.Empty;
        UnitPrice = new Money(0, "USD");
        Quantity = 0;
    }

    public Money Total() => UnitPrice.Multiply(Quantity);
}

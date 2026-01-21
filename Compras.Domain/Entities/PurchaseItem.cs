using Compras.Domain.Exceptions;
using Compras.Domain.ValueObjects;

namespace Compras.Domain.Entities;

public sealed class PurchaseItem
{
    public Guid ProductId { get; }
    public string Name { get; }
    public Money UnitPrice { get; }
    public int Quantity { get; }

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

    public Money Total() => UnitPrice.Multiply(Quantity);
}

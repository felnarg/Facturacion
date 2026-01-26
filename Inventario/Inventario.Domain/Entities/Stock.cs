namespace Inventario.Domain.Entities;

public class Stock
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Stock()
    {
    }

    public Stock(Guid productId)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Increase(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "La cantidad debe ser mayor a cero.");
        }

        Quantity += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Decrease(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "La cantidad debe ser mayor a cero.");
        }

        if (Quantity - amount < 0)
        {
            throw new InvalidOperationException("No hay stock suficiente para la operación.");
        }

        Quantity -= amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad no puede ser negativa.");
        }

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}

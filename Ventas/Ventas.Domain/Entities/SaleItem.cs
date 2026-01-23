namespace Ventas.Domain.Entities;

public class SaleItem
{
    public Guid Id { get; private set; }
    public Guid SaleId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    private SaleItem()
    {
    }

    public SaleItem(Guid productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe ser mayor a cero.");
        }

        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
    }
}

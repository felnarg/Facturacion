namespace Compras.Domain.Entities;

public class Purchase
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public string SupplierName { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Pending";
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReceivedAt { get; private set; }

    private Purchase()
    {
    }

    public Purchase(Guid productId, int quantity, string supplierName)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe ser mayor a cero.");
        }

        if (string.IsNullOrWhiteSpace(supplierName))
        {
            throw new ArgumentException("El proveedor es obligatorio.", nameof(supplierName));
        }

        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        SupplierName = supplierName.Trim();
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkReceived()
    {
        if (Status == "Received")
        {
            return;
        }

        Status = "Received";
        ReceivedAt = DateTime.UtcNow;
    }
}

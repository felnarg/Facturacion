namespace Compras.Domain.Entities;

public class Purchase
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int InternalProductCode { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Guid SupplierId { get; private set; }
    public string SupplierName { get; private set; } = string.Empty;
    public string SupplierInvoiceNumber { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Pending";
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReceivedAt { get; private set; }

    private Purchase()
    {
    }

    public Purchase(
        Guid productId,
        int internalProductCode,
        string productName,
        int quantity,
        Guid supplierId,
        string supplierName,
        string supplierInvoiceNumber)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe ser mayor a cero.");
        }

        if (string.IsNullOrWhiteSpace(supplierName))
        {
            throw new ArgumentException("El proveedor es obligatorio.", nameof(supplierName));
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("El nombre del producto es obligatorio.", nameof(productName));
        }

        if (internalProductCode <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(internalProductCode), "El código interno es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(supplierInvoiceNumber))
        {
            throw new ArgumentException("El número de factura es obligatorio.", nameof(supplierInvoiceNumber));
        }

        Id = Guid.NewGuid();
        ProductId = productId;
        InternalProductCode = internalProductCode;
        ProductName = productName.Trim();
        Quantity = quantity;
        SupplierId = supplierId;
        SupplierName = supplierName.Trim();
        SupplierInvoiceNumber = supplierInvoiceNumber.Trim();
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

namespace Catalogo.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public int SupplierProductCode { get; private set; }
    public int InternalProductCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Product()
    {
    }

    public Product(
        string name,
        string description,
        decimal price,
        int stock,
        string sku,
        int supplierProductCode,
        int internalProductCode)
    {
        Id = Guid.NewGuid();
        SetCoreFields(name, description, price, stock, sku, supplierProductCode, internalProductCode);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Update(
        string name,
        string description,
        decimal price,
        int stock,
        string sku,
        int supplierProductCode,
        int internalProductCode)
    {
        SetCoreFields(name, description, price, stock, sku, supplierProductCode, internalProductCode);
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetCoreFields(
        string name,
        string description,
        decimal price,
        int stock,
        string sku,
        int supplierProductCode,
        int internalProductCode)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(name));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "El precio no puede ser negativo.");
        }

        if (stock < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stock), "El stock no puede ser negativo.");
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("El SKU es obligatorio.", nameof(sku));
        }

        if (supplierProductCode <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(supplierProductCode), "El código del proveedor es obligatorio.");
        }

        if (internalProductCode <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(internalProductCode), "El código interno es obligatorio.");
        }

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Price = price;
        Stock = stock;
        Sku = sku.Trim();
        SupplierProductCode = supplierProductCode;
        InternalProductCode = internalProductCode;
    }
}

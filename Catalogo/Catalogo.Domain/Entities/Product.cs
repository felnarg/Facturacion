namespace Catalogo.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Product()
    {
    }

    public Product(string name, string description, decimal price, int stock, string sku)
    {
        Id = Guid.NewGuid();
        SetCoreFields(name, description, price, stock, sku);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Update(string name, string description, decimal price, int stock, string sku)
    {
        SetCoreFields(name, description, price, stock, sku);
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetCoreFields(string name, string description, decimal price, int stock, string sku)
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

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Price = price;
        Stock = stock;
        Sku = sku.Trim();
    }
}

namespace Ventas.Domain.Entities;

public class Sale
{
    private readonly List<SaleItem> _items = new();

    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<SaleItem> Items => _items;

    private Sale()
    {
    }

    public Sale(IEnumerable<SaleItem> items)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        _items = items?.ToList() ?? throw new ArgumentNullException(nameof(items));
        if (_items.Count == 0)
        {
            throw new ArgumentException("La venta debe tener al menos un item.", nameof(items));
        }
    }
}

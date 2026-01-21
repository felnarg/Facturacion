namespace Compras.Application.Models;

public sealed class PurchaseItemDto
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal Total { get; init; }
}

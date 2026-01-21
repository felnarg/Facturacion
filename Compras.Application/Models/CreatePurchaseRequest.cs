namespace Compras.Application.Models;

public sealed class CreatePurchaseRequest
{
    public Guid CustomerId { get; init; }
    public string Currency { get; init; } = "USD";
    public List<PurchaseItemRequest> Items { get; init; } = new();
}

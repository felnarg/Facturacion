namespace Compras.Application.Models;

public sealed class PurchaseDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public DateTimeOffset PurchasedAt { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public IReadOnlyCollection<PurchaseItemDto> Items { get; init; } = Array.Empty<PurchaseItemDto>();
}

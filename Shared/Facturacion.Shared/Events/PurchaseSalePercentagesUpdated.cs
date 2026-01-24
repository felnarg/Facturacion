namespace Facturacion.Shared.Events;

public sealed record SalePercentageUpdatedItem(Guid ProductId, decimal SalePercentage);

public sealed record PurchaseSalePercentagesUpdated(
    Guid PurchaseId,
    IReadOnlyList<SalePercentageUpdatedItem> Items)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

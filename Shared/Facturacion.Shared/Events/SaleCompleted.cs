namespace Facturacion.Shared.Events;

public record SaleCompleted(Guid SaleId, IReadOnlyList<SaleItem> Items)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

public record SaleItem(Guid ProductId, int Quantity);

namespace Facturacion.Shared.Events;

public record StockReceived(Guid PurchaseId, Guid ProductId, int Quantity)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

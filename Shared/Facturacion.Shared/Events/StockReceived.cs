namespace Facturacion.Shared.Events;

public record StockReceived(Guid ProductId, int Quantity)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

namespace Facturacion.Shared.Events;

public record StockDecreased(Guid ProductId, int Quantity)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

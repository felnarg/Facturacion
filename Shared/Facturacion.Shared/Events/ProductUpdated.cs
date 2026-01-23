namespace Facturacion.Shared.Events;

public record ProductUpdated(
    Guid ProductId,
    string Name,
    string Sku,
    decimal Price,
    int Stock)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

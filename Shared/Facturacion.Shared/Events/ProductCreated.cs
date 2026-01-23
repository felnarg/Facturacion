namespace Facturacion.Shared.Events;

public record ProductCreated(
    Guid ProductId,
    string Name,
    string Sku,
    decimal Price,
    int Stock)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

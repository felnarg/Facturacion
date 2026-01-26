namespace Facturacion.Shared.Events;

public record ProductUpdated(
    Guid ProductId,
    string Name,
    decimal Price,
    int Stock)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

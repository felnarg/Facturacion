namespace Facturacion.Shared.Events;

public record ProductUpdated(
    Guid ProductId,
    string Name,
    decimal Price)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

namespace Facturacion.Shared.Events;

public record ProductCreated(
    Guid ProductId,
    string Name,
    decimal Price)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

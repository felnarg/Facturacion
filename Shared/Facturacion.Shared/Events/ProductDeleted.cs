namespace Facturacion.Shared.Events;

public record ProductDeleted(Guid ProductId)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

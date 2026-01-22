namespace Catalogo.Application.Events;

public sealed record ProductUpdatedEvent(
    Guid Id,
    string Name,
    decimal Price,
    int Stock,
    string Sku,
    DateTime OccurredAt);

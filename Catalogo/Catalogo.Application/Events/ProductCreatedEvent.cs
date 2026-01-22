namespace Catalogo.Application.Events;

public sealed record ProductCreatedEvent(
    Guid Id,
    string Name,
    decimal Price,
    int Stock,
    string Sku,
    DateTime OccurredAt);

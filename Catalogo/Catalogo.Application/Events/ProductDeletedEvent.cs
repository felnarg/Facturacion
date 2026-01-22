namespace Catalogo.Application.Events;

public sealed record ProductDeletedEvent(
    Guid Id,
    DateTime OccurredAt);

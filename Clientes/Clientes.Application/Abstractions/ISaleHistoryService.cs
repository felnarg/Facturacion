namespace Clientes.Application.Abstractions;

public interface ISaleHistoryService
{
    Task AddAsync(Guid saleId, int itemsCount, DateTime occurredAt, CancellationToken cancellationToken = default);
}

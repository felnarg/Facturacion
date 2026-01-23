using Clientes.Application.Abstractions;
using Clientes.Domain.Entities;
using Clientes.Domain.Repositories;

namespace Clientes.Application.Services;

public sealed class SaleHistoryService : ISaleHistoryService
{
    private readonly ISaleHistoryRepository _repository;

    public SaleHistoryService(ISaleHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Guid saleId, int itemsCount, DateTime occurredAt, CancellationToken cancellationToken = default)
    {
        var history = new SaleHistory(saleId, itemsCount, occurredAt);
        await _repository.AddAsync(history, cancellationToken);
    }
}

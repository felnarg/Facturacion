using Clientes.Domain.Entities;

namespace Clientes.Domain.Repositories;

public interface ISaleHistoryRepository
{
    Task AddAsync(SaleHistory history, CancellationToken cancellationToken = default);
}

using Clientes.Domain.Entities;
using Clientes.Domain.Repositories;
using Clientes.Infrastructure.Data;

namespace Clientes.Infrastructure.Repositories;

public sealed class SaleHistoryRepository : ISaleHistoryRepository
{
    private readonly ClientesDbContext _dbContext;

    public SaleHistoryRepository(ClientesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(SaleHistory history, CancellationToken cancellationToken = default)
    {
        _dbContext.SaleHistories.Add(history);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

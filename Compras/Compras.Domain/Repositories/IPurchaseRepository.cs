using Compras.Domain.Entities;

namespace Compras.Domain.Repositories;

public interface IPurchaseRepository
{
    Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default);
    Task UpdateAsync(Purchase purchase, CancellationToken cancellationToken = default);
}

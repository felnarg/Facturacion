using Compras.Domain.Entities;

namespace Compras.Application.Interfaces;

public interface IPurchaseRepository
{
    Task AddAsync(Purchase purchase, CancellationToken cancellationToken);
    Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Purchase>> ListAsync(Guid? customerId, CancellationToken cancellationToken);
}

using Kardex.Domain.Entities;

namespace Kardex.Domain.Repositories;

public interface ICreditMovementRepository
{
    Task<IReadOnlyList<CreditMovement>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task AddAsync(CreditMovement movement, CancellationToken cancellationToken = default);
}

using Kardex.Domain.Entities;
using Kardex.Domain.Enums;

namespace Kardex.Domain.Repositories;

public interface ICreditAccountRepository
{
    Task<IReadOnlyList<CreditAccount>> GetAllAsync(string? search, CancellationToken cancellationToken = default);
    Task<CreditAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CreditAccount?> GetByIdentificationAsync(
        IdentificationType identificationType,
        string identificationNumber,
        CancellationToken cancellationToken = default);
    Task AddAsync(CreditAccount account, CancellationToken cancellationToken = default);
    Task UpdateAsync(CreditAccount account, CancellationToken cancellationToken = default);
    Task DeleteAsync(CreditAccount account, CancellationToken cancellationToken = default);
}

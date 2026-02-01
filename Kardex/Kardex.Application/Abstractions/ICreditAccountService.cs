using Kardex.Application.DTOs;

namespace Kardex.Application.Abstractions;

public interface ICreditAccountService
{
    Task<IReadOnlyList<CreditAccountDto>> GetAllAsync(string? search, CancellationToken cancellationToken = default);
    Task<CreditAccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CreditAccountDto> CreateAsync(CreateCreditAccountRequest request, CancellationToken cancellationToken = default);
    Task<CreditAccountDto?> UpdateAsync(Guid id, UpdateCreditAccountRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

using Kardex.Application.DTOs;

namespace Kardex.Application.Abstractions;

public interface ICreditSaleService
{
    Task<CreditMovementDto> RegisterAsync(CreateCreditSaleRequest request, CancellationToken cancellationToken = default);
    Task<CreditMovementDto> RegisterByAccountAsync(Guid accountId, CreateCreditSaleByAccountRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CreditMovementDto>> GetMovementsAsync(Guid accountId, CancellationToken cancellationToken = default);
}

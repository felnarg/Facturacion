using Ventas.Application.DTOs;

namespace Ventas.Application.Abstractions;

public interface ISaleService
{
    Task<IReadOnlyList<SaleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SaleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SaleDto> CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default);
}

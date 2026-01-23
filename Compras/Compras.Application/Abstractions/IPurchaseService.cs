using Compras.Application.DTOs;

namespace Compras.Application.Abstractions;

public interface IPurchaseService
{
    Task<IReadOnlyList<PurchaseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PurchaseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PurchaseDto> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken = default);
    Task<PurchaseDto?> MarkReceivedAsync(Guid id, CancellationToken cancellationToken = default);
}

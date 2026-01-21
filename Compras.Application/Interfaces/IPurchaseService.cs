using Compras.Application.Models;

namespace Compras.Application.Interfaces;

public interface IPurchaseService
{
    Task<PurchaseDto> CreateAsync(CreatePurchaseRequest request, CancellationToken cancellationToken);
    Task<PurchaseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PurchaseDto>> ListAsync(Guid? customerId, CancellationToken cancellationToken);
}

using System.Collections.Concurrent;
using Compras.Application.Interfaces;
using Compras.Domain.Entities;

namespace Compras.Infrastructure.Repositories;

public sealed class InMemoryPurchaseRepository : IPurchaseRepository
{
    private readonly ConcurrentDictionary<Guid, Purchase> _storage = new();

    public Task AddAsync(Purchase purchase, CancellationToken cancellationToken)
    {
        _storage[purchase.Id] = purchase;
        return Task.CompletedTask;
    }

    public Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _storage.TryGetValue(id, out var purchase);
        return Task.FromResult(purchase);
    }

    public Task<IReadOnlyCollection<Purchase>> ListAsync(Guid? customerId, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Purchase> purchases = _storage.Values
            .Where(purchase => customerId is null || purchase.CustomerId == customerId)
            .OrderByDescending(purchase => purchase.PurchasedAt)
            .ToArray();

        return Task.FromResult(purchases);
    }
}

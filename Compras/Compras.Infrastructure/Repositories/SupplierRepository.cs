using Compras.Domain.Entities;
using Compras.Domain.Repositories;
using Compras.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Compras.Infrastructure.Repositories;

public sealed class SupplierRepository : ISupplierRepository
{
    private readonly ComprasDbContext _dbContext;

    public SupplierRepository(ComprasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Supplier>> GetAllAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Suppliers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(supplier =>
                supplier.Name.ToLower().Contains(term) ||
                supplier.ContactName.ToLower().Contains(term) ||
                supplier.Email.ToLower().Contains(term) ||
                supplier.Phone.ToLower().Contains(term) ||
                supplier.Address.ToLower().Contains(term));
        }

        return await query.OrderBy(supplier => supplier.Name).ToListAsync(cancellationToken);
    }

    public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Suppliers.AsNoTracking()
            .FirstOrDefaultAsync(supplier => supplier.Id == id, cancellationToken);
    }

    public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        _dbContext.Suppliers.Add(supplier);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        _dbContext.Suppliers.Update(supplier);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        _dbContext.Suppliers.Remove(supplier);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

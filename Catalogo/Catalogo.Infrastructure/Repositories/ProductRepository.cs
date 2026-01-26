using Catalogo.Domain.Entities;
using Catalogo.Domain.Repositories;
using Catalogo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly CatalogoDbContext _dbContext;

    public ProductRepository(CatalogoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(product =>
                product.Name.ToLower().Contains(term) ||
                product.Description.ToLower().Contains(term) ||
                product.SupplierProductCode.ToString().Contains(term) ||
                product.InternalProductCode.ToString().Contains(term));
        }

        return await query.OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

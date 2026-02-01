using Kardex.Domain.Entities;
using Kardex.Domain.Enums;
using Kardex.Domain.Repositories;
using Kardex.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kardex.Infrastructure.Repositories;

public sealed class CreditAccountRepository : ICreditAccountRepository
{
    private readonly KardexDbContext _dbContext;

    public CreditAccountRepository(KardexDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CreditAccount>> GetAllAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.CreditAccounts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(account =>
                account.CustomerName.ToLower().Contains(term) ||
                account.IdentificationNumber.ToLower().Contains(term) ||
                account.IdentificationType.ToString().ToLower().Contains(term));
        }

        return await query
            .OrderBy(account => account.CustomerName)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<CreditAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CreditAccounts
            .Include(account => account.Movements)
            .FirstOrDefaultAsync(account => account.Id == id, cancellationToken);
    }

    public async Task<CreditAccount?> GetByIdentificationAsync(
        IdentificationType identificationType,
        string identificationNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.CreditAccounts
            .FirstOrDefaultAsync(
                account => account.IdentificationType == identificationType &&
                           account.IdentificationNumber == identificationNumber,
                cancellationToken);
    }

    public async Task AddAsync(CreditAccount account, CancellationToken cancellationToken = default)
    {
        _dbContext.CreditAccounts.Add(account);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(CreditAccount account, CancellationToken cancellationToken = default)
    {
        _dbContext.CreditAccounts.Update(account);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(CreditAccount account, CancellationToken cancellationToken = default)
    {
        _dbContext.CreditAccounts.Remove(account);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

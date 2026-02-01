using Kardex.Domain.Entities;
using Kardex.Domain.Repositories;
using Kardex.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kardex.Infrastructure.Repositories;

public sealed class CreditMovementRepository : ICreditMovementRepository
{
    private readonly KardexDbContext _dbContext;

    public CreditMovementRepository(KardexDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CreditMovement>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CreditMovements
            .Where(movement => movement.CreditAccountId == accountId)
            .OrderByDescending(movement => movement.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CreditMovement movement, CancellationToken cancellationToken = default)
    {
        _dbContext.CreditMovements.Add(movement);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

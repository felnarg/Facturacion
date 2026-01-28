using Microsoft.EntityFrameworkCore;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Repositories;
using Usuarios.Infrastructure.Data;

namespace Usuarios.Infrastructure.Repositories;

public sealed class PermissionRepository : IPermissionRepository
{
    private readonly UsuariosDbContext _dbContext;

    public PermissionRepository(UsuariosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();
        return await _dbContext.Permissions.FirstOrDefaultAsync(p => p.Code == normalizedCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions.AsNoTracking()
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByModuleAsync(string module, CancellationToken cancellationToken = default)
    {
        var normalizedModule = module.Trim().ToLowerInvariant();
        return await _dbContext.Permissions.AsNoTracking()
            .Where(p => p.Module == normalizedModule)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions.AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken = default)
    {
        var normalizedCodes = codes.Select(c => c.Trim().ToLowerInvariant()).ToList();
        return await _dbContext.Permissions.AsNoTracking()
            .Where(p => normalizedCodes.Contains(p.Code))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _dbContext.Permissions.Add(permission);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Permission> permissions, CancellationToken cancellationToken = default)
    {
        _dbContext.Permissions.AddRange(permissions);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();
        return await _dbContext.Permissions.AnyAsync(p => p.Code == normalizedCode, cancellationToken);
    }
}

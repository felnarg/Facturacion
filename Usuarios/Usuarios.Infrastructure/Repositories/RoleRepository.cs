using Microsoft.EntityFrameworkCore;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Repositories;
using Usuarios.Infrastructure.Data;

namespace Usuarios.Infrastructure.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly UsuariosDbContext _dbContext;

    public RoleRepository(UsuariosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(role => role.Id == id, cancellationToken);
    }

    public async Task<Role?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles
            .Include(role => role.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(role => role.Id == id, cancellationToken);
    }

    public async Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();
        return await _dbContext.Roles
            .Include(role => role.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(role => role.Code == normalizedCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.AsNoTracking()
            .Include(role => role.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .OrderByDescending(role => role.HierarchyLevel)
            .ThenBy(role => role.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.AsNoTracking()
            .Include(role => role.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(role => role.IsActive)
            .OrderByDescending(role => role.HierarchyLevel)
            .ThenBy(role => role.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.AsNoTracking()
            .Include(role => role.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(role => ids.Contains(role.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        _dbContext.Roles.Update(role);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();
        return await _dbContext.Roles.AnyAsync(role => role.Code == normalizedCode, cancellationToken);
    }

    public async Task DeleteAsync(Role role, CancellationToken cancellationToken = default)
    {
        // Las relaciones RolePermission se eliminan en cascada por la configuración de EF
        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasUsersAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles.AnyAsync(ur => ur.RoleId == roleId, cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Repositories;
using Usuarios.Infrastructure.Data;

namespace Usuarios.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly UsuariosDbContext _dbContext;

    public UserRepository(UsuariosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdWithRolesAndPermissionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(role => role.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _dbContext.Users
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByEmailWithRolesAndPermissionsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _dbContext.Users
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(role => role.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AsNoTracking()
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AsNoTracking()
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(user => user.IsActive)
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AsNoTracking()
            .Include(user => user.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(user => user.UserRoles.Any(ur => ur.RoleId == roleId))
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await _dbContext.Users.AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
    }
}


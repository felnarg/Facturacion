using Usuarios.Domain.Entities;

namespace Usuarios.Domain.Repositories;

/// <summary>
/// Repositorio para la gestión de permisos
/// </summary>
public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByModuleAsync(string module, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken = default);
    Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Permission> permissions, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
}

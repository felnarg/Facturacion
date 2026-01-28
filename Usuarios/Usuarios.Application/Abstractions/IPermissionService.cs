using Usuarios.Application.DTOs;

namespace Usuarios.Application.Abstractions;

/// <summary>
/// Servicio para gestión de permisos
/// </summary>
public interface IPermissionService
{
    Task<PermissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PermissionDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PermissionsByModuleDto>> GetAllGroupedByModuleAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PermissionDto>> GetByModuleAsync(string module, CancellationToken cancellationToken = default);
}

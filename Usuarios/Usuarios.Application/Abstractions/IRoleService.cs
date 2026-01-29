using Usuarios.Application.DTOs;

namespace Usuarios.Application.Abstractions;

/// <summary>
/// Servicio para gestión de roles
/// </summary>
public interface IRoleService
{
    Task<RoleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoleDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleDto>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task<RoleDto> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken = default);
    Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task AssignPermissionsAsync(Guid roleId, AssignPermissionsRequest request, CancellationToken cancellationToken = default);
    Task RemovePermissionAsync(Guid roleId, string permissionCode, CancellationToken cancellationToken = default);
}

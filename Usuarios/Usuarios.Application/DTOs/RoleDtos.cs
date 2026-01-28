namespace Usuarios.Application.DTOs;

// ═══════════════════════════════════════════════════════════════════════════
// DTOs de Roles
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Información de un rol para respuestas API
/// </summary>
public sealed record RoleDto(
    Guid Id,
    string Code,
    string Name,
    string Description,
    int HierarchyLevel,
    bool IsActive,
    bool IsSystemRole,
    IReadOnlyList<PermissionDto> Permissions);

/// <summary>
/// Request para crear un nuevo rol
/// </summary>
public sealed record CreateRoleRequest(
    string Code,
    string Name,
    string Description,
    int HierarchyLevel,
    IReadOnlyList<string> PermissionCodes);

/// <summary>
/// Request para actualizar un rol existente
/// </summary>
public sealed record UpdateRoleRequest(
    string? Name,
    string? Description,
    int? HierarchyLevel);

/// <summary>
/// Request para asignar permisos a un rol
/// </summary>
public sealed record AssignPermissionsRequest(IReadOnlyList<string> PermissionCodes);

namespace Usuarios.Application.DTOs;

// ═══════════════════════════════════════════════════════════════════════════
// DTOs de Permisos
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Información de un permiso para respuestas API
/// </summary>
public sealed record PermissionDto(
    Guid Id,
    string Code,
    string Name,
    string Description,
    string Module);

/// <summary>
/// Permisos agrupados por módulo
/// </summary>
public sealed record PermissionsByModuleDto(
    string Module,
    IReadOnlyList<PermissionDto> Permissions);

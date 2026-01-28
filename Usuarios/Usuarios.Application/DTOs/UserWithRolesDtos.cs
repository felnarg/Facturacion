namespace Usuarios.Application.DTOs;

// ═══════════════════════════════════════════════════════════════════════════
// DTOs extendidos de Usuario
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Información completa de un usuario con roles
/// </summary>
public sealed record UserWithRolesDto(
    Guid Id,
    string Email,
    string Name,
    string? PhoneNumber,
    string? ProfilePictureUrl,
    bool IsActive,
    DateTime? LastLoginAt,
    IReadOnlyList<string> Roles,
    DateTime CreatedAt,
    DateTime UpdatedAt);

/// <summary>
/// Solicitud para asignar roles a un usuario
/// </summary>
public sealed record AssignUserRolesRequest(IReadOnlyList<string> RoleCodes);

/// <summary>
/// Request para asignar roles a un usuario
/// </summary>
public sealed record AssignRolesRequest(IReadOnlyList<string> RoleCodes);

/// <summary>
/// Request para actualizar perfil de usuario
/// </summary>
public sealed record UpdateUserProfileRequest(
    string? Name,
    string? PhoneNumber,
    string? ProfilePictureUrl);

/// <summary>
/// Request para cambiar contraseña
/// </summary>
public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);

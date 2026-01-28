namespace Usuarios.Application.DTOs;

/// <summary>
/// Respuesta de autenticación con token JWT y permisos
/// </summary>
public sealed record AuthResponse(
    Guid UserId, 
    string Email, 
    string Name,
    string Token,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);


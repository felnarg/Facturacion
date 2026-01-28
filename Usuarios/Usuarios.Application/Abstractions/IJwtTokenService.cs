namespace Usuarios.Application.Abstractions;

/// <summary>
/// Servicio para generación y validación de tokens JWT
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Genera un token JWT básico con userId y email
    /// </summary>
    string GenerateToken(Guid userId, string email);
    
    /// <summary>
    /// Genera un token JWT completo con roles y permisos
    /// </summary>
    string GenerateToken(Guid userId, string email, string userName, IEnumerable<string> roles, IEnumerable<string> permissions);
}


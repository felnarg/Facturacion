using Microsoft.AspNetCore.Mvc;
using Usuarios.Application.Abstractions;
using Usuarios.Application.DTOs;

namespace Usuarios.API.Controllers;

/// <summary>
/// Controlador de autenticación para registro y login de usuarios
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema
    /// </summary>
    /// <param name="request">Datos de registro</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Token JWT con roles y permisos del usuario</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Inicia sesión de un usuario existente
    /// </summary>
    /// <param name="request">Credenciales de login</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Token JWT con roles y permisos del usuario</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            if (response is null)
            {
                return Unauthorized(new { message = "Credenciales inválidas." });
            }

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            // Cuenta bloqueada o desactivada
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }
}


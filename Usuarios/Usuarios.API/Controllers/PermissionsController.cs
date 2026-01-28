using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Usuarios.Application.Abstractions;
using Usuarios.Application.DTOs;

namespace Usuarios.API.Controllers;

/// <summary>
/// Controlador para consulta de permisos del sistema RBAC
/// </summary>
[ApiController]
[Route("api/permissions")]
[Produces("application/json")]
[Authorize]
public sealed class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// Obtiene todos los permisos del sistema
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:roles.read")]
    [ProducesResponseType(typeof(IReadOnlyList<PermissionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PermissionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var permissions = await _permissionService.GetAllAsync(cancellationToken);
        return Ok(permissions);
    }

    /// <summary>
    /// Obtiene todos los permisos agrupados por módulo
    /// </summary>
    [HttpGet("grouped")]
    [Authorize(Policy = "RequirePermission:roles.read")]
    [ProducesResponseType(typeof(IReadOnlyList<PermissionsByModuleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PermissionsByModuleDto>>> GetAllGroupedByModule(CancellationToken cancellationToken)
    {
        var permissions = await _permissionService.GetAllGroupedByModuleAsync(cancellationToken);
        return Ok(permissions);
    }

    /// <summary>
    /// Obtiene un permiso por su ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "RequirePermission:roles.read")]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var permission = await _permissionService.GetByIdAsync(id, cancellationToken);
        if (permission is null)
        {
            return NotFound(new { message = $"No se encontró el permiso con ID '{id}'." });
        }
        return Ok(permission);
    }

    /// <summary>
    /// Obtiene un permiso por su código
    /// </summary>
    [HttpGet("code/{code}")]
    [Authorize(Policy = "RequirePermission:roles.read")]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionDto>> GetByCode(string code, CancellationToken cancellationToken)
    {
        var permission = await _permissionService.GetByCodeAsync(code, cancellationToken);
        if (permission is null)
        {
            return NotFound(new { message = $"No se encontró el permiso con código '{code}'." });
        }
        return Ok(permission);
    }

    /// <summary>
    /// Obtiene todos los permisos de un módulo específico
    /// </summary>
    [HttpGet("module/{module}")]
    [Authorize(Policy = "RequirePermission:roles.read")]
    [ProducesResponseType(typeof(IReadOnlyList<PermissionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PermissionDto>>> GetByModule(string module, CancellationToken cancellationToken)
    {
        var permissions = await _permissionService.GetByModuleAsync(module, cancellationToken);
        return Ok(permissions);
    }
}

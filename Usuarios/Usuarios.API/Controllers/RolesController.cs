using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Usuarios.Application.Abstractions;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Constants;

namespace Usuarios.API.Controllers;

/// <summary>
/// Controlador para gestión de roles del sistema RBAC
/// </summary>
[ApiController]
[Route("api/roles")]
[Produces("application/json")]
[Authorize]
public sealed class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Obtiene todos los roles del sistema
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:roles.read")]
    [ProducesResponseType(typeof(IReadOnlyList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllAsync(cancellationToken);
        return Ok(roles);
    }

    /// <summary>
    /// Obtiene todos los roles activos del sistema
    /// </summary>
    [HttpGet("active")]
    [Authorize(Policy = "RequirePermission:roles.read")]
    [ProducesResponseType(typeof(IReadOnlyList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAllActive(CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllActiveAsync(cancellationToken);
        return Ok(roles);
    }

    /// <summary>
    /// Obtiene un rol por su ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "RequirePermission:roles.read")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetByIdAsync(id, cancellationToken);
        if (role is null)
        {
            return NotFound(new { message = $"No se encontró el rol con ID '{id}'." });
        }
        return Ok(role);
    }

    /// <summary>
    /// Obtiene un rol por su código
    /// </summary>
    [HttpGet("code/{code}")]
    [Authorize(Policy = "RequirePermission:roles.read")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> GetByCode(string code, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetByCodeAsync(code, cancellationToken);
        if (role is null)
        {
            return NotFound(new { message = $"No se encontró el rol con código '{code}'." });
        }
        return Ok(role);
    }

    /// <summary>
    /// Crea un nuevo rol
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:roles.manage")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RoleDto>> Create(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un rol existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "RequirePermission:roles.manage")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> Update(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleService.UpdateAsync(id, request, cancellationToken);
            return Ok(role);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Activa un rol
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [Authorize(Policy = "RequirePermission:roles.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _roleService.ActivateAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Desactiva un rol
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Policy = "RequirePermission:roles.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _roleService.DeactivateAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("sistema"))
            {
                return BadRequest(new { message = ex.Message });
            }
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Asigna permisos a un rol
    /// </summary>
    [HttpPost("{id:guid}/permissions")]
    [Authorize(Policy = "RequirePermission:permissions.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignPermissions(Guid id, AssignPermissionsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _roleService.AssignPermissionsAsync(id, request, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un permiso de un rol
    /// </summary>
    [HttpDelete("{id:guid}/permissions/{permissionCode}")]
    [Authorize(Policy = "RequirePermission:permissions.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemovePermission(Guid id, string permissionCode, CancellationToken cancellationToken)
    {
        try
        {
            await _roleService.RemovePermissionAsync(id, permissionCode, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un rol permanentemente
    /// </summary>
    /// <remarks>
    /// No se pueden eliminar roles del sistema ni roles con usuarios asignados.
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequirePermission:roles.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _roleService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("sistema") || ex.Message.Contains("usuarios asignados"))
            {
                return BadRequest(new { message = ex.Message });
            }
            return NotFound(new { message = ex.Message });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Usuarios.Application.Abstractions;
using Usuarios.Application.DTOs;

namespace Usuarios.API.Controllers;

/// <summary>
/// Controlador para gestión de usuarios
/// </summary>
[ApiController]
[Route("api/users")]
[Produces("application/json")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Obtiene todos los usuarios con sus roles
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequirePermission:users.read")]
    public async Task<ActionResult<IReadOnlyList<UserWithRolesDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "RequirePermission:users.read")]
    public async Task<ActionResult<UserWithRolesDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequirePermission:users.manage")]
    public async Task<ActionResult<UserWithRolesDto>> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Activa un usuario
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [Authorize(Policy = "RequirePermission:users.manage")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.ActivateAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Desactiva un usuario
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Policy = "RequirePermission:users.manage")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.DeactivateAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Asigna roles a un usuario
    /// </summary>
    [HttpPost("{id:guid}/roles")]
    [Authorize(Policy = "RequirePermission:users.roles.manage")]
    public async Task<IActionResult> AssignRoles(Guid id, AssignUserRolesRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.AssignRolesAsync(id, request, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error assigning roles", detail = ex.Message, stack = ex.StackTrace });
        }
    }

    /// <summary>
    /// Elimina un usuario permanentemente
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequirePermission:users.delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

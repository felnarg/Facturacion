using Usuarios.Application.Abstractions;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Repositories;

namespace Usuarios.Application.Services;

public sealed class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public RoleService(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(id, cancellationToken);
        return role is null ? null : MapToDto(role);
    }

    public async Task<RoleDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByCodeAsync(code, cancellationToken);
        return role is null ? null : MapToDto(role);
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        return roles.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetAllActiveAsync(cancellationToken);
        return roles.Select(MapToDto).ToList();
    }

    public async Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        if (await _roleRepository.ExistsAsync(request.Code, cancellationToken))
        {
            throw new InvalidOperationException($"Ya existe un rol con el código '{request.Code}'.");
        }

        var role = new Role(request.Code, request.Name, request.Description, request.HierarchyLevel);

        // Asignar permisos si se proporcionaron
        if (request.PermissionCodes.Any())
        {
            var permissions = await _permissionRepository.GetByCodesAsync(request.PermissionCodes, cancellationToken);
            foreach (var permission in permissions)
            {
                role.AddPermission(new RolePermission(role.Id, permission.Id));
            }
        }

        await _roleRepository.AddAsync(role, cancellationToken);
        
        // Recargar para obtener las relaciones completas
        var createdRole = await _roleRepository.GetByIdWithPermissionsAsync(role.Id, cancellationToken);
        return MapToDto(createdRole!);
    }

    public async Task<RoleDto> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró el rol con ID '{id}'.");

        role.Update(request.Name, request.Description, request.HierarchyLevel);
        await _roleRepository.UpdateAsync(role, cancellationToken);

        return MapToDto(role);
    }

    public async Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró el rol con ID '{id}'.");

        role.Activate();
        await _roleRepository.UpdateAsync(role, cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró el rol con ID '{id}'.");

        role.Deactivate();
        await _roleRepository.UpdateAsync(role, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró el rol con ID '{id}'.");

        // Validación de negocio: no permitir eliminar roles del sistema
        if (role.IsSystemRole)
        {
            throw new InvalidOperationException("No se puede eliminar un rol del sistema.");
        }

        // Validación de negocio: no permitir eliminar roles con usuarios asignados
        if (await _roleRepository.HasUsersAsync(id, cancellationToken))
        {
            throw new InvalidOperationException("No se puede eliminar un rol que tiene usuarios asignados. Primero reasigne los usuarios a otro rol.");
        }

        await _roleRepository.DeleteAsync(role, cancellationToken);
    }

    public async Task AssignPermissionsAsync(Guid roleId, AssignPermissionsRequest request, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(roleId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró el rol con ID '{roleId}'.");

        var permissions = await _permissionRepository.GetByCodesAsync(request.PermissionCodes, cancellationToken);
        
        foreach (var permission in permissions)
        {
            role.AddPermission(new RolePermission(roleId, permission.Id));
        }

        await _roleRepository.UpdateAsync(role, cancellationToken);
    }

    public async Task RemovePermissionAsync(Guid roleId, string permissionCode, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdWithPermissionsAsync(roleId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró el rol con ID '{roleId}'.");

        var permission = await _permissionRepository.GetByCodeAsync(permissionCode, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontró el permiso con código '{permissionCode}'.");

        role.RemovePermission(permission.Id);
        await _roleRepository.UpdateAsync(role, cancellationToken);
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto(
            role.Id,
            role.Code,
            role.Name,
            role.Description,
            role.HierarchyLevel,
            role.IsActive,
            role.IsSystemRole,
            role.RolePermissions.Select(rp => new PermissionDto(
                rp.Permission.Id,
                rp.Permission.Code,
                rp.Permission.Name,
                rp.Permission.Description,
                rp.Permission.Module
            )).ToList()
        );
    }
}

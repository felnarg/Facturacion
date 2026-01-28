using Usuarios.Application.Abstractions;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Repositories;

namespace Usuarios.Application.Services;

public sealed class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<PermissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetByIdAsync(id, cancellationToken);
        return permission is null ? null : new PermissionDto(
            permission.Id,
            permission.Code,
            permission.Name,
            permission.Description,
            permission.Module
        );
    }

    public async Task<PermissionDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetByCodeAsync(code, cancellationToken);
        return permission is null ? null : new PermissionDto(
            permission.Id,
            permission.Code,
            permission.Name,
            permission.Description,
            permission.Module
        );
    }

    public async Task<IReadOnlyList<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        return permissions.Select(p => new PermissionDto(
            p.Id,
            p.Code,
            p.Name,
            p.Description,
            p.Module
        )).ToList();
    }

    public async Task<IReadOnlyList<PermissionsByModuleDto>> GetAllGroupedByModuleAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);
        
        return permissions
            .GroupBy(p => p.Module)
            .OrderBy(g => g.Key)
            .Select(g => new PermissionsByModuleDto(
                g.Key,
                g.Select(p => new PermissionDto(
                    p.Id,
                    p.Code,
                    p.Name,
                    p.Description,
                    p.Module
                )).ToList()
            ))
            .ToList();
    }

    public async Task<IReadOnlyList<PermissionDto>> GetByModuleAsync(string module, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetByModuleAsync(module, cancellationToken);
        return permissions.Select(p => new PermissionDto(
            p.Id,
            p.Code,
            p.Name,
            p.Description,
            p.Module
        )).ToList();
    }
}

using System.Security.Cryptography;
using System.Text;
using Usuarios.Application.Abstractions;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Repositories;

namespace Usuarios.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IRoleRepository _roleRepository;

    public UserService(IUserRepository repository, IRoleRepository roleRepository)
    {
        _repository = repository;
        _roleRepository = roleRepository;
    }

    public async Task<IReadOnlyList<UserWithRolesDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetAllWithRolesAsync(cancellationToken);
        return users.Select(MapWithRoles).ToList();
    }

    public async Task<UserWithRolesDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdWithRolesAsync(id, cancellationToken);
        return user is null ? null : MapWithRoles(user);
    }

    public async Task<UserWithRolesDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var passwordHash = ComputeHash(request.Password);
        var user = new User(request.Name, request.Email, passwordHash);
        await _repository.AddAsync(user, cancellationToken);
        return MapWithRoles(user);
    }

    public async Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("Usuario no encontrado.");
        }

        user.Activate();
        await _repository.UpdateAsync(user, cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("Usuario no encontrado.");
        }

        user.Deactivate();
        await _repository.UpdateAsync(user, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("Usuario no encontrado.");
        }

        await _repository.DeleteAsync(user, cancellationToken);
    }

    public async Task AssignRolesAsync(Guid userId, AssignUserRolesRequest request, CancellationToken cancellationToken = default)
    {
        // Verificar que el usuario existe
        var user = await _repository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException($"Usuario con ID '{userId}' no encontrado.");
        }

        // Validar y obtener IDs de los roles solicitados
        var roleIds = new List<Guid>();
        foreach (var code in request.RoleCodes)
        {
            var role = await _roleRepository.GetByCodeAsync(code, cancellationToken);
            if (role is null)
            {
                throw new InvalidOperationException($"El rol '{code}' no existe.");
            }
            roleIds.Add(role.Id);
        }

        // Delegar la sincronización al repositorio
        await _repository.SyncUserRolesAsync(userId, roleIds, cancellationToken);
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }

    private static UserWithRolesDto MapWithRoles(User user)
    {
        var roles = user.GetActiveRoleCodes().ToList();
        
        return new UserWithRolesDto(
            user.Id,
            user.Email,
            user.Name,
            user.PhoneNumber,
            user.ProfilePictureUrl,
            user.IsActive,
            user.LastLoginAt,
            roles,
            user.CreatedAt,
            user.UpdatedAt);
    }
}

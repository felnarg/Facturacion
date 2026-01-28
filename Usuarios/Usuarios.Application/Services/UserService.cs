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

    public UserService(IUserRepository repository)
    {
        _repository = repository;
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

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

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetAllAsync(cancellationToken);
        return users.Select(Map).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : Map(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var passwordHash = ComputeHash(request.Password);
        var user = new User(request.Name, request.Email, passwordHash);
        await _repository.AddAsync(user, cancellationToken);
        return Map(user);
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }

    private static UserDto Map(User user)
    {
        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt,
            user.UpdatedAt);
    }
}

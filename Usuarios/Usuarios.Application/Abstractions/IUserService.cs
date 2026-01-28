using Usuarios.Application.DTOs;

namespace Usuarios.Application.Abstractions;

public interface IUserService
{
    Task<IReadOnlyList<UserWithRolesDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserWithRolesDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserWithRolesDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}

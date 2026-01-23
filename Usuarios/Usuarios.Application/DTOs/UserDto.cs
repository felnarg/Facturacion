namespace Usuarios.Application.DTOs;

public sealed record UserDto(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt,
    DateTime UpdatedAt);

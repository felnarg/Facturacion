namespace Clientes.Application.DTOs;

public sealed record CustomerDto(
    Guid Id,
    string Name,
    string Email,
    int Points,
    DateTime CreatedAt,
    DateTime UpdatedAt);

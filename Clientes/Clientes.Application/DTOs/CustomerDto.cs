using Clientes.Domain.Entities;

namespace Clientes.Application.DTOs;

public sealed record CustomerDto(
    Guid Id,
    string Name,
    string Email,
    string City,
    string Phone,
    string Address,
    CustomerType Type,
    IdentificationType IdentificationType,
    string IdentificationNumber,
    int Points,
    DateTime CreatedAt,
    DateTime UpdatedAt);

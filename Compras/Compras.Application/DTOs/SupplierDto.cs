namespace Compras.Application.DTOs;

public sealed record SupplierDto(
    Guid Id,
    string Name,
    string ContactName,
    string Phone,
    string Email,
    string Address);

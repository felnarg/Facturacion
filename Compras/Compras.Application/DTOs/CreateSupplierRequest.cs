namespace Compras.Application.DTOs;

public sealed record CreateSupplierRequest(
    string Name,
    string ContactName,
    string Phone,
    string Email,
    string Address);

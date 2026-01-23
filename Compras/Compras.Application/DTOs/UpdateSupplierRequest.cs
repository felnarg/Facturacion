namespace Compras.Application.DTOs;

public sealed record UpdateSupplierRequest(
    string Name,
    string ContactName,
    string Phone,
    string Email,
    string Address);

namespace Compras.Application.DTOs;

public sealed record CreatePurchaseRequest(
    Guid ProductId,
    int Quantity,
    Guid SupplierId);

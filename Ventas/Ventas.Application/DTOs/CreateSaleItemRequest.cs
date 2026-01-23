namespace Ventas.Application.DTOs;

public sealed record CreateSaleItemRequest(Guid ProductId, int Quantity);

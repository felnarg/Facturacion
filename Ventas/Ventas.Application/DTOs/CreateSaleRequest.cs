namespace Ventas.Application.DTOs;

public sealed record CreateSaleRequest(IReadOnlyList<CreateSaleItemRequest> Items);

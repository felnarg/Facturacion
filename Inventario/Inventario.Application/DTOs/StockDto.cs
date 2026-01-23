namespace Inventario.Application.DTOs;

public sealed record StockDto(
    Guid ProductId,
    int Quantity,
    DateTime CreatedAt,
    DateTime UpdatedAt);

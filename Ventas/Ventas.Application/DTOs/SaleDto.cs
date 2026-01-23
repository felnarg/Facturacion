namespace Ventas.Application.DTOs;

public sealed record SaleDto(
    Guid Id,
    DateTime CreatedAt,
    IReadOnlyList<SaleItemDto> Items);

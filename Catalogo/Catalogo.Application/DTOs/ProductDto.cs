namespace Catalogo.Application.DTOs;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Sku,
    DateTime CreatedAt,
    DateTime UpdatedAt);

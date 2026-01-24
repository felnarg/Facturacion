namespace Catalogo.Application.DTOs;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Sku,
    int SupplierProductCode,
    int InternalProductCode,
    decimal SalePercentage,
    DateTime CreatedAt,
    DateTime UpdatedAt);

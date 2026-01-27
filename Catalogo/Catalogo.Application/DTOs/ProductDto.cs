namespace Catalogo.Application.DTOs;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int SupplierProductCode,
    int InternalProductCode,
    decimal SalePercentage,
    decimal ConsumptionTaxPercentage,
    decimal WholesaleSalePercentage,
    decimal SpecialSalePercentage,
    decimal Iva,
    DateTime CreatedAt,
    DateTime UpdatedAt);

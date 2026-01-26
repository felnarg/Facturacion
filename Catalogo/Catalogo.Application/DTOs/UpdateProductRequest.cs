namespace Catalogo.Application.DTOs;

public sealed record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    int SupplierProductCode,
    int InternalProductCode,
    decimal SalePercentage = 30m,
    decimal ConsumptionTaxPercentage = 0m,
    decimal WholesaleSalePercentage = 25m,
    decimal SpecialSalePercentage = 20m,
    decimal Iva = 19m);

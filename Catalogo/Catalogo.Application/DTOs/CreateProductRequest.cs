namespace Catalogo.Application.DTOs;

public sealed class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int SupplierProductCode { get; set; }
    public int InternalProductCode { get; set; }
    public decimal SalePercentage { get; set; } = 30m;
    public decimal ConsumptionTaxPercentage { get; set; } = 0m;
    public decimal WholesaleSalePercentage { get; set; } = 25m;
    public decimal SpecialSalePercentage { get; set; } = 20m;
    public decimal Iva { get; set; } = 19m;
}

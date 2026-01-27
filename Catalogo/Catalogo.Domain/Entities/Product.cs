namespace Catalogo.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int SupplierProductCode { get; private set; }
    public int InternalProductCode { get; private set; }
    public decimal SalePercentage { get; private set; }
    public decimal ConsumptionTaxPercentage { get; private set; }
    public decimal WholesaleSalePercentage { get; private set; }
    public decimal SpecialSalePercentage { get; private set; }
    public decimal Iva { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Product()
    {
    }

    public Product(
        string name,
        string description,
        decimal price,
        int supplierProductCode,
        int internalProductCode,
        decimal salePercentage,
        decimal consumptionTaxPercentage,
        decimal wholesaleSalePercentage,
        decimal specialSalePercentage,
        decimal iva)
    {
        Id = Guid.NewGuid();
        SetCoreFields(
            name,
            description,
            price,
            supplierProductCode,
            internalProductCode,
            salePercentage,
            consumptionTaxPercentage,
            wholesaleSalePercentage,
            specialSalePercentage,
            iva);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Update(
        string name,
        string description,
        decimal price,
        int supplierProductCode,
        int internalProductCode,
        decimal salePercentage,
        decimal consumptionTaxPercentage,
        decimal wholesaleSalePercentage,
        decimal specialSalePercentage,
        decimal iva)
    {
        SetCoreFields(
            name,
            description,
            price,
            supplierProductCode,
            internalProductCode,
            salePercentage,
            consumptionTaxPercentage,
            wholesaleSalePercentage,
            specialSalePercentage,
            iva);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSalePercentage(decimal salePercentage)
    {
        ValidatePercentage(salePercentage, nameof(salePercentage));
        SalePercentage = salePercentage;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetCoreFields(
        string name,
        string description,
        decimal price,
        int supplierProductCode,
        int internalProductCode,
        decimal salePercentage,
        decimal consumptionTaxPercentage,
        decimal wholesaleSalePercentage,
        decimal specialSalePercentage,
        decimal iva)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre es obligatorio.", nameof(name));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "El precio no puede ser negativo.");
        }

        if (supplierProductCode <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(supplierProductCode), "El código del proveedor es obligatorio.");
        }

        if (internalProductCode <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(internalProductCode), "El código interno es obligatorio.");
        }

        ValidatePercentage(salePercentage, nameof(salePercentage));
        ValidatePercentage(consumptionTaxPercentage, nameof(consumptionTaxPercentage));
        ValidatePercentage(wholesaleSalePercentage, nameof(wholesaleSalePercentage));
        ValidatePercentage(specialSalePercentage, nameof(specialSalePercentage));
        ValidatePercentage(iva, nameof(iva));

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Price = price;
        SupplierProductCode = supplierProductCode;
        InternalProductCode = internalProductCode;
        SalePercentage = salePercentage;
        ConsumptionTaxPercentage = consumptionTaxPercentage;
        WholesaleSalePercentage = wholesaleSalePercentage;
        SpecialSalePercentage = specialSalePercentage;
        Iva = iva;
    }

    private static void ValidatePercentage(decimal value, string paramName)
    {
        if (value < 0 || value > 100)
        {
            throw new ArgumentOutOfRangeException(paramName, "El porcentaje debe estar entre 0 y 100.");
        }
    }
}

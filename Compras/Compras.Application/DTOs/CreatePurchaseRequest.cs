namespace Compras.Application.DTOs;

public sealed record CreatePurchaseRequest(
    Guid ProductId,
    int InternalProductCode,
    string ProductName,
    int Quantity,
    Guid SupplierId,
    string SupplierInvoiceNumber);

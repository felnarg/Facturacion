namespace Compras.Application.DTOs;

public sealed record PurchaseDto(
    Guid Id,
    Guid ProductId,
    int InternalProductCode,
    string ProductName,
    Guid SupplierId,
    int Quantity,
    string SupplierName,
    string SupplierInvoiceNumber,
    string Status,
    DateTime CreatedAt,
    DateTime? ReceivedAt);

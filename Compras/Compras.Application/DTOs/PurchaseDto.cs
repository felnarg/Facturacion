namespace Compras.Application.DTOs;

public sealed record PurchaseDto(
    Guid Id,
    Guid ProductId,
    Guid SupplierId,
    int Quantity,
    string SupplierName,
    string Status,
    DateTime CreatedAt,
    DateTime? ReceivedAt);

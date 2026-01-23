namespace Compras.Application.DTOs;

public sealed record PurchaseDto(
    Guid Id,
    Guid ProductId,
    int Quantity,
    string SupplierName,
    string Status,
    DateTime CreatedAt,
    DateTime? ReceivedAt);

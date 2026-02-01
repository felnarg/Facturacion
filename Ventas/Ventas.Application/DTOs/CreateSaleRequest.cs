namespace Ventas.Application.DTOs;

public sealed record CreateSaleRequest(
    IReadOnlyList<CreateSaleItemRequest> Items,
    string? PaymentMethod,
    decimal? TotalAmount,
    string? CustomerIdentificationType,
    string? CustomerIdentification);

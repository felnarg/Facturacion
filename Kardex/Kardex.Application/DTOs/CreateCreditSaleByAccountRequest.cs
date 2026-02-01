namespace Kardex.Application.DTOs;

public sealed record CreateCreditSaleByAccountRequest(
    decimal Amount,
    Guid? SaleId,
    DateTime? OccurredOn);

using Kardex.Domain.Enums;

namespace Kardex.Application.DTOs;

public sealed record CreditMovementDto(
    Guid Id,
    Guid CreditAccountId,
    Guid? SaleId,
    decimal Amount,
    DateTime DueDate,
    CreditMovementStatus Status,
    DateTime CreatedAt);

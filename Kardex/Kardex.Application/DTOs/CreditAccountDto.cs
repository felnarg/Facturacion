using Kardex.Domain.Enums;

namespace Kardex.Application.DTOs;

public sealed record CreditAccountDto(
    Guid Id,
    string CustomerName,
    IdentificationType IdentificationType,
    string IdentificationNumber,
    decimal CreditLimit,
    int PaymentTermDays,
    decimal CurrentBalance,
    decimal AvailableCredit,
    DateTime CreatedAt,
    DateTime UpdatedAt);

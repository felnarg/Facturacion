using Kardex.Domain.Enums;

namespace Kardex.Application.DTOs;

public sealed record CreateCreditSaleRequest(
    IdentificationType IdentificationType,
    string IdentificationNumber,
    decimal Amount,
    Guid? SaleId,
    DateTime? OccurredOn);

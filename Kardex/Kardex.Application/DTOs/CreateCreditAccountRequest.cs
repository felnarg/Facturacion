using Kardex.Domain.Enums;

namespace Kardex.Application.DTOs;

public sealed record CreateCreditAccountRequest(
    string CustomerName,
    IdentificationType IdentificationType,
    string IdentificationNumber,
    decimal CreditLimit,
    int PaymentTermDays);

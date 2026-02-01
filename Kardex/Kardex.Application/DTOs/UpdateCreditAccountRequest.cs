namespace Kardex.Application.DTOs;

public sealed record UpdateCreditAccountRequest(
    string CustomerName,
    decimal CreditLimit,
    int PaymentTermDays);

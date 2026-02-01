using Clientes.Domain.Entities;

namespace Clientes.Application.DTOs;

public sealed record UpdateCustomerRequest(
    string Name,
    string Email,
    string City,
    string Phone,
    string Address,
    CustomerType Type,
    IdentificationType IdentificationType,
    string IdentificationNumber,
    bool IsCreditApproved,
    decimal ApprovedCreditLimit,
    int ApprovedPaymentTermDays);

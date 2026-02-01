namespace Facturacion.Shared.Events;

public record CustomerCreditApproved(
    Guid CustomerId,
    string CustomerName,
    string IdentificationType,
    string IdentificationNumber,
    decimal CreditLimit,
    int PaymentTermDays)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

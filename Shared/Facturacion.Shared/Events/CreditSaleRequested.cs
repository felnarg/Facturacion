namespace Facturacion.Shared.Events;

public record CreditSaleRequested(
    Guid SaleId,
    string IdentificationType,
    string IdentificationNumber,
    decimal Amount,
    DateTime OccurredOn)
    : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

namespace Facturacion.Shared.Events;

/// <summary>
/// Evento publicado por FacturacionElectronica cuando una factura ha sido
/// transmitida a la DIAN (aceptada o rechazada). Es consumido por Notificaciones
/// para enviar el email con resultado al cliente.
/// </summary>
public record FacturaTransmitidaDian(
    Guid DocumentoId,
    string NumeroDocumento,
    string EmisorNit,
    string EmisorNombre,
    string EmisorEmail,
    string ClienteIdentificacion,
    string ClienteNombre,
    string ClienteEmail,
    decimal Subtotal,
    decimal TotalImpuestos,
    decimal Total,
    DateTime FechaEmision,
    string Cufe,
    bool Aceptado,
    string RespuestaDian,
    string? TrackId
) : IntegrationEvent(Guid.NewGuid(), DateTime.UtcNow);

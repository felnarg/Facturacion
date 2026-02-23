namespace FacturacionElectronica.Domain.Interfaces;

/// <summary>
/// Respuesta devuelta por los Web Services de la DIAN tras transmitir un documento.
/// Código de estado: "00" = validado, "01" = rechazado, "02" = en proceso.
/// </summary>
public record RespuestaDianWs(
    bool Aceptado,
    string CodigoEstado,        // "00", "01", "02", "03", "04"
    string Descripcion,
    string? TrackId,            // ID de rastreo asíncrono DIAN
    string? ZipKey,             // Clave del ZIP enviado
    DateTime FechaRespuesta
);

/// <summary>
/// Transmite documentos electrónicos a la DIAN mediante SOAP/WS-Security.
/// Implementa SendBillAsync y GetStatus según el Suplemento J y el Anexo Técnico DIAN.
/// URLs: pruebas → vpfe-hab.dian.gov.co | producción → vpfe.dian.gov.co
/// </summary>
public interface IDianSoapService
{
    /// <summary>
    /// Envía el XML firmado a la DIAN (SendBillAsync). 
    /// El XML se empaqueta en ZIP codificado en Base64 antes del envío.
    /// </summary>
    Task<RespuestaDianWs> TransmitirDocumentoAsync(
        string xmlFirmado,
        string numeroDocumento,
        string nitEmisor,
        bool esAmbientePruebas);

    /// <summary>
    /// Envía al ambiente de habilitación DIAN (SendTestSetAsync).
    /// Requiere un TestSetId válido asignado por la DIAN.
    /// </summary>
    Task<RespuestaDianWs> TransmitirDocumentoPruebasAsync(
        string xmlFirmado,
        string numeroDocumento,
        string testSetId);

    /// <summary>
    /// Consulta el estado de un documento previamente enviado (GetStatus).
    /// </summary>
    Task<RespuestaDianWs> ConsultarEstadoAsync(string trackId, bool esAmbientePruebas);
}

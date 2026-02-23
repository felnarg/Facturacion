using FacturacionElectronica.Domain.Entities;

namespace FacturacionElectronica.Domain.Interfaces;

/// <summary>
/// Genera el XML UBL 2.1 según el estándar técnico colombiano exigido por la DIAN.
/// Más información: Anexo técnico DIAN, sección 6.1 (Invoice).
/// </summary>
public interface IXmlGeneratorService
{
    /// <summary>
    /// Construye el XML UBL 2.1 del documento electrónico.
    /// El XML generado aún no está firmado; debe pasar por IFirmaDigitalService.
    /// </summary>
    Task<string> GenerarXmlUbl21Async(
        DocumentoElectronico documento,
        Emisor emisor,
        Cliente cliente,
        NumeracionDocumento numeracion);
}

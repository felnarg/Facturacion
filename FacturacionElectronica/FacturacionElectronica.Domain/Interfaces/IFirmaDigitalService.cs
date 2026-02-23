using FacturacionElectronica.Domain.Entities;

namespace FacturacionElectronica.Domain.Interfaces;

/// <summary>
/// Resultado de la operación de firma del documento XML.
/// </summary>
public record ResultadoFirma(
    string XmlFirmado,
    string Cufe   // SHA-384 hex (96 chars) según suplemento B DIAN
);

/// <summary>
/// Firma digitalmente el XML UBL 2.1 usando XAdES-EPES con el certificado
/// del emisor (PKCS#12 en Base64). Calcula el CUFE (Suplemento B DIAN).
/// </summary>
public interface IFirmaDigitalService
{
    /// <summary>
    /// Firma el XML del documento y calcula el CUFE correspondiente.
    /// </summary>
    /// <param name="xmlContent">XML generado por IXmlGeneratorService (sin firma)</param>
    /// <param name="emisor">Emisor con CertificadoDigital (Base64 PKCS#12) y ClaveCertificado</param>
    /// <param name="documento">Documento para calcular el CUFE</param>
    /// <param name="cliente">Cliente para el NIT adquiriente en CUFE</param>
    /// <param name="esAmbientePruebas">true → ProfileExecutionID=1, false → 2</param>
    Task<ResultadoFirma> FirmarDocumentoAsync(
        string xmlContent,
        Emisor emisor,
        DocumentoElectronico documento,
        Cliente cliente,
        bool esAmbientePruebas);
}

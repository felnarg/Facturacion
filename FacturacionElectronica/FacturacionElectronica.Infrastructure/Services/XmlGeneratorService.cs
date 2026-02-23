using FacturacionElectronica.Domain.Entities;
using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.Interfaces;
using System.Text;
using System.Xml;

namespace FacturacionElectronica.Infrastructure.Services;

/// <summary>
/// Genera el XML UBL 2.1 para la DIAN Colombia según el Anexo Técnico.
/// Referencia: Skills/03-Estructura-Invoice.md y Skills/18-Suplemento-K-UBLExtensions.md
/// </summary>
public sealed class XmlGeneratorService : IXmlGeneratorService
{
    // Namespaces UBL 2.1 requeridos por la DIAN
    private const string NsInvoice = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
    private const string NsCac     = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
    private const string NsCbc     = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
    private const string NsExt     = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2";
    private const string NsSts     = "dian.gov:co:facturaelectronica:Structures-2-1";
    private const string NsDs      = "http://www.w3.org/2000/09/xmldsig#";
    private const string NsXades   = "http://uri.etsi.org/01903/v1.3.2#";

    public Task<string> GenerarXmlUbl21Async(
        DocumentoElectronico documento,
        Emisor emisor,
        Cliente cliente,
        NumeracionDocumento numeracion)
    {
        var xml = new StringBuilder();
        xml.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        xml.AppendLine($"""<Invoice xmlns="{NsInvoice}" """);
        xml.AppendLine($"""         xmlns:cac="{NsCac}" """);
        xml.AppendLine($"""         xmlns:cbc="{NsCbc}" """);
        xml.AppendLine($"""         xmlns:ext="{NsExt}" """);
        xml.AppendLine($"""         xmlns:sts="{NsSts}" """);
        xml.AppendLine($"""         xmlns:ds="{NsDs}" """);
        xml.AppendLine($"""         xmlns:xades="{NsXades}">""");

        // ── UBLExtensions (DianExtensions + Signature placeholder) ──────────────
        xml.AppendLine(GenerarUblExtensions(emisor, numeracion));

        // ── Encabezado del documento ─────────────────────────────────────────────
        var fechaEmision = documento.FechaEmision.ToString("yyyy-MM-dd");
        var horaEmision  = documento.FechaEmision.ToString("HH:mm:ss") + "-05:00";

        xml.AppendLine("  <cbc:UBLVersionID>UBL 2.1</cbc:UBLVersionID>");
        xml.AppendLine($"  <cbc:CustomizationID>{ObtenerCustomizationId(documento.TipoDocumento)}</cbc:CustomizationID>");
        xml.AppendLine("  <cbc:ProfileID>DIAN 2.1: Factura Electrónica de Venta</cbc:ProfileID>");
        xml.AppendLine("  <cbc:ProfileExecutionID>1</cbc:ProfileExecutionID>"); // se actualiza con el CUFE
        xml.AppendLine($"  <cbc:ID>{documento.NumeroDocumento}</cbc:ID>");
        xml.AppendLine($"  <cbc:UUID schemeID=\"1\" schemeName=\"CUFE-SHA384\">CUFE_PLACEHOLDER</cbc:UUID>");
        xml.AppendLine($"  <cbc:IssueDate>{fechaEmision}</cbc:IssueDate>");
        xml.AppendLine($"  <cbc:IssueTime>{horaEmision}</cbc:IssueTime>");
        xml.AppendLine($"  <cbc:InvoiceTypeCode>{ObtenerCodigoTipoFactura(documento.TipoDocumento)}</cbc:InvoiceTypeCode>");
        xml.AppendLine("  <cbc:DocumentCurrencyCode>COP</cbc:DocumentCurrencyCode>");
        xml.AppendLine($"  <cbc:LineCountNumeric>{documento.Items.Count}</cbc:LineCountNumeric>");

        // ── Emisor (AccountingSupplierParty) ────────────────────────────────────
        xml.AppendLine(GenerarEmisor(emisor));

        // ── Cliente (AccountingCustomerParty) ───────────────────────────────────
        xml.AppendLine(GenerarCliente(cliente));

        // ── Impuestos globales (TaxTotal) ───────────────────────────────────────
        foreach (var impuesto in documento.Impuestos)
        {
            xml.AppendLine(GenerarImpuesto(impuesto));
        }

        // ── Totales monetarios (LegalMonetaryTotal) ──────────────────────────────
        xml.AppendLine(GenerarTotales(documento));

        // ── Líneas de detalle (InvoiceLine) ──────────────────────────────────────
        foreach (var item in documento.Items)
        {
            xml.AppendLine(GenerarLinea(item, documento.Impuestos));
        }

        xml.AppendLine("</Invoice>");
        return Task.FromResult(xml.ToString());
    }

    // ── Helpers privados ─────────────────────────────────────────────────────────

    private static string GenerarUblExtensions(Emisor emisor, NumeracionDocumento numeracion)
    {
        var sb = new StringBuilder();
        sb.AppendLine("  <ext:UBLExtensions>");
        sb.AppendLine("    <ext:UBLExtension>");
        sb.AppendLine("      <ext:ExtensionContent>");
        sb.AppendLine("        <sts:DianExtensions>");

        // InvoiceControl
        sb.AppendLine("          <sts:InvoiceControl>");
        sb.AppendLine($"            <sts:InvoiceAuthorization>{emisor.ResolucionHabilitacion}</sts:InvoiceAuthorization>");
        sb.AppendLine("            <sts:AuthorizationPeriod>");
        sb.AppendLine($"              <cbc:StartDate>{numeracion.FechaAutorizacion:yyyy-MM-dd}</cbc:StartDate>");
        sb.AppendLine($"              <cbc:EndDate>{numeracion.FechaVencimiento:yyyy-MM-dd}</cbc:EndDate>");
        sb.AppendLine("            </sts:AuthorizationPeriod>");
        sb.AppendLine("            <sts:AuthorizedInvoices>");
        sb.AppendLine($"              <sts:Prefix>{numeracion.Prefijo}</sts:Prefix>");
        sb.AppendLine($"              <sts:From>{numeracion.NumeroInicial}</sts:From>");
        sb.AppendLine($"              <sts:To>{numeracion.NumeroFinal}</sts:To>");
        sb.AppendLine("            </sts:AuthorizedInvoices>");
        sb.AppendLine("          </sts:InvoiceControl>");

        // InvoiceSource: Colombia
        sb.AppendLine("          <sts:InvoiceSource>");
        sb.AppendLine("""            <cbc:IdentificationCode listAgencyID="6" listAgencyName="United Nations Economic Commission for Europe" listSchemeURI="urn:oasis:names:specification:ubl:codelist:gc:CountryIdentificationCode-2.1">CO</cbc:IdentificationCode>""");
        sb.AppendLine("          </sts:InvoiceSource>");

        // SoftwareProvider
        sb.AppendLine("          <sts:SoftwareProvider>");
        sb.AppendLine($"""            <sts:ProviderID schemeAgencyID="195" schemeAgencyName="CO, DIAN" schemeID="4" schemeName="31">{emisor.Codigo}</sts:ProviderID>""");
        sb.AppendLine($"""            <sts:softwareID schemeAgencyID="195" schemeAgencyName="CO, DIAN">{emisor.SoftwareId}</sts:softwareID>""");
        sb.AppendLine("          </sts:SoftwareProvider>");

        // SoftwareSecurityCode (PIN SHA-384)
        var securityCode = CalcularSoftwareSecurityCode(emisor.SoftwareId, emisor.PinSoftware);
        sb.AppendLine($"""          <sts:SoftwareSecurityCode schemeAgencyID="195" schemeAgencyName="CO, DIAN">{securityCode}</sts:SoftwareSecurityCode>""");

        // AuthorizationProvider (DIAN: NIT 800197268)
        sb.AppendLine("          <sts:AuthorizationProvider>");
        sb.AppendLine("""            <sts:AuthorizationProviderID schemeAgencyID="195" schemeAgencyName="CO, DIAN" schemeID="4" schemeName="31">800197268</sts:AuthorizationProviderID>""");
        sb.AppendLine("          </sts:AuthorizationProvider>");

        // QRCode placeholder (se actualiza después de calcular el CUFE)
        sb.AppendLine("          <sts:QRCode>QR_PLACEHOLDER</sts:QRCode>");

        sb.AppendLine("        </sts:DianExtensions>");
        sb.AppendLine("      </ext:ExtensionContent>");
        sb.AppendLine("    </ext:UBLExtension>");

        // Segundo UBLExtension: Firma digital (placeholder, se completa en FirmaDigitalService)
        sb.AppendLine("    <ext:UBLExtension>");
        sb.AppendLine("      <ext:ExtensionContent>");
        sb.AppendLine("        <ds:Signature Id=\"xmldsig-SIGNATURE_ID\"/>");
        sb.AppendLine("      </ext:ExtensionContent>");
        sb.AppendLine("    </ext:UBLExtension>");

        sb.AppendLine("  </ext:UBLExtensions>");
        return sb.ToString();
    }

    private static string GenerarEmisor(Emisor emisor)
    {
        var tipoOrg = emisor.TipoPersona == TipoPersona.PersonaJuridica ? "1" : "2";
        var taxLevel = ObtenerTaxLevelCode(emisor.ResponsabilidadFiscal);
        var sb = new StringBuilder();

        sb.AppendLine("  <cac:AccountingSupplierParty>");
        sb.AppendLine($"    <cbc:AdditionalAccountID>{tipoOrg}</cbc:AdditionalAccountID>");
        sb.AppendLine("    <cac:Party>");
        sb.AppendLine("      <cac:PartyName>");
        sb.AppendLine($"        <cbc:Name>{EscapeXml(emisor.NombreComercial ?? emisor.RazonSocial)}</cbc:Name>");
        sb.AppendLine("      </cac:PartyName>");

        // Dirección física
        sb.AppendLine("      <cac:PhysicalLocation>");
        sb.AppendLine("        <cac:Address>");
        sb.AppendLine($"          <cbc:ID>{emisor.Direccion.CodigoPostal ?? "110111"}</cbc:ID>");
        sb.AppendLine($"          <cbc:CityName>{EscapeXml(emisor.Direccion.Ciudad)}</cbc:CityName>");
        sb.AppendLine($"          <cbc:CountrySubentity>{EscapeXml(emisor.Direccion.Departamento)}</cbc:CountrySubentity>");
        sb.AppendLine($"          <cbc:CountrySubentityCode>11</cbc:CountrySubentityCode>");
        sb.AppendLine("          <cac:AddressLine>");
        sb.AppendLine($"            <cbc:Line>{EscapeXml(emisor.Direccion.Calle)} {EscapeXml(emisor.Direccion.Numero)}</cbc:Line>");
        sb.AppendLine("          </cac:AddressLine>");
        sb.AppendLine("          <cac:Country>");
        sb.AppendLine("            <cbc:IdentificationCode>CO</cbc:IdentificationCode>");
        sb.AppendLine("          </cac:Country>");
        sb.AppendLine("        </cac:Address>");
        sb.AppendLine("      </cac:PhysicalLocation>");

        // PartyTaxScheme
        sb.AppendLine("      <cac:PartyTaxScheme>");
        sb.AppendLine($"        <cbc:RegistrationName>{EscapeXml(emisor.RazonSocial)}</cbc:RegistrationName>");
        sb.AppendLine($"""        <cbc:CompanyID schemeAgencyID="195" schemeAgencyName="CO, DIAN" schemeID="9" schemeName="31">{emisor.Codigo}</cbc:CompanyID>""");
        sb.AppendLine($"        <cbc:TaxLevelCode listName=\"48\">{taxLevel}</cbc:TaxLevelCode>");
        sb.AppendLine("        <cac:RegistrationAddress>");
        sb.AppendLine($"          <cbc:ID>{emisor.Direccion.CodigoPostal ?? "110111"}</cbc:ID>");
        sb.AppendLine($"          <cbc:CityName>{EscapeXml(emisor.Direccion.Ciudad)}</cbc:CityName>");
        sb.AppendLine($"          <cbc:CountrySubentity>{EscapeXml(emisor.Direccion.Departamento)}</cbc:CountrySubentity>");
        sb.AppendLine($"          <cbc:CountrySubentityCode>11</cbc:CountrySubentityCode>");
        sb.AppendLine("          <cac:AddressLine>");
        sb.AppendLine($"            <cbc:Line>{EscapeXml(emisor.Direccion.Calle)} {EscapeXml(emisor.Direccion.Numero)}</cbc:Line>");
        sb.AppendLine("          </cac:AddressLine>");
        sb.AppendLine("          <cac:Country>");
        sb.AppendLine("            <cbc:IdentificationCode>CO</cbc:IdentificationCode>");
        sb.AppendLine("          </cac:Country>");
        sb.AppendLine("        </cac:RegistrationAddress>");
        sb.AppendLine("        <cac:TaxScheme>");
        sb.AppendLine("          <cbc:ID>01</cbc:ID>");
        sb.AppendLine("          <cbc:Name>IVA</cbc:Name>");
        sb.AppendLine("        </cac:TaxScheme>");
        sb.AppendLine("      </cac:PartyTaxScheme>");

        // PartyLegalEntity
        sb.AppendLine("      <cac:PartyLegalEntity>");
        sb.AppendLine($"        <cbc:RegistrationName>{EscapeXml(emisor.RazonSocial)}</cbc:RegistrationName>");
        sb.AppendLine($"""        <cbc:CompanyID schemeAgencyID="195" schemeAgencyName="CO, DIAN" schemeID="9" schemeName="31">{emisor.Codigo}</cbc:CompanyID>""");
        sb.AppendLine("      </cac:PartyLegalEntity>");

        // Contacto
        if (!string.IsNullOrWhiteSpace(emisor.Contacto?.Email))
        {
            sb.AppendLine("      <cac:Contact>");
            sb.AppendLine($"        <cbc:ElectronicMail>{EscapeXml(emisor.Contacto.Email)}</cbc:ElectronicMail>");
            sb.AppendLine("      </cac:Contact>");
        }

        sb.AppendLine("    </cac:Party>");
        sb.AppendLine("  </cac:AccountingSupplierParty>");
        return sb.ToString();
    }

    private static string GenerarCliente(Cliente cliente)
    {
        var esConsumidorFinal = cliente.EsConsumidorFinal();
        var tipoOrg = cliente.TipoPersona == TipoPersona.PersonaJuridica ? "1" : "2";
        var taxLevel = ObtenerTaxLevelCode(cliente.ResponsabilidadFiscal);
        var schemeName = cliente.TipoPersona == TipoPersona.PersonaJuridica ? "31" : "13";
        var schemeId   = cliente.TipoPersona == TipoPersona.PersonaJuridica ? "9"  : "5";
        var sb = new StringBuilder();

        sb.AppendLine("  <cac:AccountingCustomerParty>");
        sb.AppendLine($"    <cbc:AdditionalAccountID>{tipoOrg}</cbc:AdditionalAccountID>");
        sb.AppendLine("    <cac:Party>");
        sb.AppendLine("      <cac:PartyTaxScheme>");
        sb.AppendLine($"        <cbc:RegistrationName>{EscapeXml(esConsumidorFinal ? "Consumidor Final" : cliente.RazonSocial)}</cbc:RegistrationName>");
        sb.AppendLine($"""        <cbc:CompanyID schemeAgencyID="195" schemeAgencyName="CO, DIAN" schemeID="{schemeId}" schemeName="{schemeName}">{cliente.Codigo}</cbc:CompanyID>""");
        sb.AppendLine($"        <cbc:TaxLevelCode listName=\"48\">{(esConsumidorFinal ? "R-99-PN" : taxLevel)}</cbc:TaxLevelCode>");
        sb.AppendLine("        <cac:RegistrationAddress>");
        sb.AppendLine($"          <cbc:ID>{cliente.Direccion.CodigoPostal ?? "110111"}</cbc:ID>");
        sb.AppendLine($"          <cbc:CityName>{EscapeXml(cliente.Direccion.Ciudad)}</cbc:CityName>");
        sb.AppendLine($"          <cbc:CountrySubentity>{EscapeXml(cliente.Direccion.Departamento)}</cbc:CountrySubentity>");
        sb.AppendLine($"          <cbc:CountrySubentityCode>{cliente.CodigoDepartamento}</cbc:CountrySubentityCode>");
        sb.AppendLine("          <cac:AddressLine>");
        sb.AppendLine($"            <cbc:Line>{EscapeXml(cliente.Direccion.Calle)} {EscapeXml(cliente.Direccion.Numero)}</cbc:Line>");
        sb.AppendLine("          </cac:AddressLine>");
        sb.AppendLine("          <cac:Country>");
        sb.AppendLine("            <cbc:IdentificationCode>CO</cbc:IdentificationCode>");
        sb.AppendLine("          </cac:Country>");
        sb.AppendLine("        </cac:RegistrationAddress>");
        sb.AppendLine("        <cac:TaxScheme>");
        sb.AppendLine("          <cbc:ID>01</cbc:ID>");
        sb.AppendLine("          <cbc:Name>IVA</cbc:Name>");
        sb.AppendLine("        </cac:TaxScheme>");
        sb.AppendLine("      </cac:PartyTaxScheme>");

        if (!esConsumidorFinal)
        {
            sb.AppendLine("      <cac:PartyLegalEntity>");
            sb.AppendLine($"        <cbc:RegistrationName>{EscapeXml(cliente.RazonSocial)}</cbc:RegistrationName>");
            sb.AppendLine($"""        <cbc:CompanyID schemeAgencyID="195" schemeAgencyName="CO, DIAN" schemeID="{schemeId}" schemeName="{schemeName}">{cliente.Codigo}</cbc:CompanyID>""");
            sb.AppendLine("      </cac:PartyLegalEntity>");
        }

        if (!string.IsNullOrWhiteSpace(cliente.Contacto?.Email))
        {
            sb.AppendLine("      <cac:Contact>");
            sb.AppendLine($"        <cbc:ElectronicMail>{EscapeXml(cliente.Contacto.Email)}</cbc:ElectronicMail>");
            sb.AppendLine("      </cac:Contact>");
        }

        sb.AppendLine("    </cac:Party>");
        sb.AppendLine("  </cac:AccountingCustomerParty>");
        return sb.ToString();
    }

    private static string GenerarImpuesto(ImpuestoDocumento impuesto)
    {
        var (idTributo, nombreTributo) = ObtenerCodigoImpuesto(impuesto.TipoImpuesto);
        return $"""
          <cac:TaxTotal>
            <cbc:TaxAmount currencyID="COP">{impuesto.Valor.Valor:F2}</cbc:TaxAmount>
            <cac:TaxSubtotal>
              <cbc:TaxableAmount currencyID="COP">{impuesto.BaseGravable.Valor:F2}</cbc:TaxableAmount>
              <cbc:TaxAmount currencyID="COP">{impuesto.Valor.Valor:F2}</cbc:TaxAmount>
              <cac:TaxCategory>
                <cbc:Percent>{impuesto.Porcentaje:F2}</cbc:Percent>
                <cac:TaxScheme>
                  <cbc:ID>{idTributo}</cbc:ID>
                  <cbc:Name>{nombreTributo}</cbc:Name>
                </cac:TaxScheme>
              </cac:TaxCategory>
            </cac:TaxSubtotal>
          </cac:TaxTotal>
        """;
    }

    private static string GenerarTotales(DocumentoElectronico documento)
    {
        return $"""
          <cac:LegalMonetaryTotal>
            <cbc:LineExtensionAmount currencyID="COP">{documento.Subtotal.Valor:F2}</cbc:LineExtensionAmount>
            <cbc:TaxExclusiveAmount currencyID="COP">{documento.Subtotal.Valor:F2}</cbc:TaxExclusiveAmount>
            <cbc:TaxInclusiveAmount currencyID="COP">{documento.Total.Valor:F2}</cbc:TaxInclusiveAmount>
            <cbc:AllowanceTotalAmount currencyID="COP">{documento.TotalDescuentos.Valor:F2}</cbc:AllowanceTotalAmount>
            <cbc:PayableAmount currencyID="COP">{documento.Total.Valor:F2}</cbc:PayableAmount>
          </cac:LegalMonetaryTotal>
        """;
    }

    private static string GenerarLinea(ItemDocumento item, IEnumerable<ImpuestoDocumento> impuestosGlobales)
    {
        // Busca el IVA para la línea (simplificado: proporcional a la base)
        var ivaGlobal = impuestosGlobales.FirstOrDefault(i => i.TipoImpuesto == TipoImpuesto.IVA);
        var porcentajeIva = ivaGlobal?.Porcentaje ?? 0;
        var baseLinea = item.ValorTotal.Valor;
        var ivaLinea  = Math.Round(baseLinea * porcentajeIva / 100, 2);

        var sb = new StringBuilder();
        sb.AppendLine($"  <cac:InvoiceLine>");
        sb.AppendLine($"    <cbc:ID>{item.Orden}</cbc:ID>");
        sb.AppendLine($"""    <cbc:InvoicedQuantity unitCode="C62">{item.Cantidad:F2}</cbc:InvoicedQuantity>""");
        sb.AppendLine($"""    <cbc:LineExtensionAmount currencyID="COP">{baseLinea:F2}</cbc:LineExtensionAmount>""");

        if (ivaLinea > 0)
        {
            sb.AppendLine("    <cac:TaxTotal>");
            sb.AppendLine($"""      <cbc:TaxAmount currencyID="COP">{ivaLinea:F2}</cbc:TaxAmount>""");
            sb.AppendLine("      <cac:TaxSubtotal>");
            sb.AppendLine($"""        <cbc:TaxableAmount currencyID="COP">{baseLinea:F2}</cbc:TaxableAmount>""");
            sb.AppendLine($"""        <cbc:TaxAmount currencyID="COP">{ivaLinea:F2}</cbc:TaxAmount>""");
            sb.AppendLine("        <cac:TaxCategory>");
            sb.AppendLine($"          <cbc:Percent>{porcentajeIva:F2}</cbc:Percent>");
            sb.AppendLine("          <cac:TaxScheme>");
            sb.AppendLine("            <cbc:ID>01</cbc:ID>");
            sb.AppendLine("            <cbc:Name>IVA</cbc:Name>");
            sb.AppendLine("          </cac:TaxScheme>");
            sb.AppendLine("        </cac:TaxCategory>");
            sb.AppendLine("      </cac:TaxSubtotal>");
            sb.AppendLine("    </cac:TaxTotal>");
        }

        sb.AppendLine("    <cac:Item>");
        sb.AppendLine($"      <cbc:Description>{EscapeXml(item.Descripcion)}</cbc:Description>");
        if (!string.IsNullOrWhiteSpace(item.Codigo))
        {
            sb.AppendLine($"      <cac:SellersItemIdentification>");
            sb.AppendLine($"        <cbc:ID>{EscapeXml(item.Codigo)}</cbc:ID>");
            sb.AppendLine($"      </cac:SellersItemIdentification>");
        }
        sb.AppendLine("    </cac:Item>");
        sb.AppendLine("    <cac:Price>");
        sb.AppendLine($"""      <cbc:PriceAmount currencyID="COP">{item.ValorUnitario.Valor:F2}</cbc:PriceAmount>""");
        sb.AppendLine("    </cac:Price>");
        sb.AppendLine("  </cac:InvoiceLine>");
        return sb.ToString();
    }

    // ── Utilidades ───────────────────────────────────────────────────────────────

    /// <summary>
    /// SoftwareSecurityCode = SHA-384(SoftwareId + PinSoftware)
    /// </summary>
    private static string CalcularSoftwareSecurityCode(string softwareId, string pinSoftware)
    {
        var texto = softwareId + pinSoftware;
        var bytes = System.Security.Cryptography.SHA384.HashData(Encoding.UTF8.GetBytes(texto));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string ObtenerCustomizationId(TipoDocumento tipo) => tipo switch
    {
        TipoDocumento.FacturaElectronica            => "10",
        TipoDocumento.FacturaElectronicaExportacion => "09",
        TipoDocumento.FacturaElectronicaContingencia=> "10",
        _                                           => "10"
    };

    private static string ObtenerCodigoTipoFactura(TipoDocumento tipo) => tipo switch
    {
        TipoDocumento.FacturaElectronica            => "01",
        TipoDocumento.FacturaElectronicaExportacion => "02",
        TipoDocumento.FacturaElectronicaContingencia=> "03",
        _                                           => "01"
    };

    private static (string Id, string Nombre) ObtenerCodigoImpuesto(TipoImpuesto tipo) => tipo switch
    {
        TipoImpuesto.IVA            => ("01", "IVA"),
        TipoImpuesto.ICA            => ("04", "ICA"),
        TipoImpuesto.INPP           => ("05", "INPP"),
        TipoImpuesto.IBUA           => ("10", "IBUA"),
        TipoImpuesto.ICIU           => ("20", "ICIU"),
        TipoImpuesto.ICL            => ("21", "ICL"),
        TipoImpuesto.RetencionIVA   => ("07", "ReteIVA"),
        TipoImpuesto.RetencionFuente=> ("06", "ReteFuente"),
        _                           => ("01", "IVA")
    };

    private static string ObtenerTaxLevelCode(TipoResponsabilidadFiscal resp) => resp switch
    {
        TipoResponsabilidadFiscal.ResponsableIVA    => "O-47",
        TipoResponsabilidadFiscal.NoResponsableIVA  => "R-99-PN",
        TipoResponsabilidadFiscal.GranContribuyente => "O-23",
        TipoResponsabilidadFiscal.Autorretenedor    => "O-15",
        TipoResponsabilidadFiscal.RegimenSimple     => "O-13",
        _                                           => "O-47"
    };

    private static string EscapeXml(string? s) =>
        string.IsNullOrWhiteSpace(s) ? string.Empty
        : s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
}

namespace FacturacionElectronica.Domain.Enums
{
    public enum TipoDocumento
    {
        FacturaElectronica = 1,
        FacturaElectronicaExportacion = 2,
        FacturaElectronicaContingencia = 3,
        NotaCredito = 4,
        NotaDebito = 5,
        DocumentoEquivalente = 6
    }

    public enum TipoPersona
    {
        PersonaNatural = 1,
        PersonaJuridica = 2
    }

    public enum TipoResponsabilidadFiscal
    {
        ResponsableIVA = 1,
        NoResponsableIVA = 2,
        GranContribuyente = 3,
        Autorretenedor = 4,
        AgenteRetenedorIVA = 5,
        RegimenSimple = 6
    }

    public enum TipoImpuesto
    {
        IVA = 1,
        ICA = 2,
        INPP = 3,
        IBUA = 4,
        ICIU = 5,
        ICL = 6,
        RetencionIVA = 7,
        RetencionFuente = 8
    }

    public enum EstadoDocumento
    {
        Pendiente = 1,
        Generado = 2,
        Firmado = 3,
        Validado = 4,
        Transmitido = 5,
        Aceptado = 6,
        Rechazado = 7,
        Contingencia = 8
    }

    public enum UnidadMedida
    {
        Unidad = 1,
        Kilogramo = 2,
        Litro = 3,
        Metro = 4,
        MetroCuadrado = 5,
        MetroCubico = 6,
        Paquete = 7,
        Caja = 8
    }
}
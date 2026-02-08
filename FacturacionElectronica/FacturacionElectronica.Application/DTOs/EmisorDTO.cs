using FacturacionElectronica.Domain.Enums;

namespace FacturacionElectronica.Application.DTOs
{
    public class EmisorDTO
    {
        public string Nit { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public TipoPersona TipoPersona { get; set; }
        public TipoResponsabilidadFiscal ResponsabilidadFiscal { get; set; }
        public DireccionDTO Direccion { get; set; }
        public ContactoDTO Contacto { get; set; }
        public string RegistroMercantil { get; set; }
        public string ResolucionHabilitacion { get; set; }
        public DateTime FechaHabilitacion { get; set; }
        public string SoftwareId { get; set; }
        public string PinSoftware { get; set; }
        public List<NumeracionDocumentoDTO> Numeraciones { get; set; } = new();
    }

    public class CrearEmisorDTO
    {
        public string Nit { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public TipoPersona TipoPersona { get; set; }
        public TipoResponsabilidadFiscal ResponsabilidadFiscal { get; set; }
        public DireccionDTO Direccion { get; set; }
        public ContactoDTO Contacto { get; set; }
        public string RegistroMercantil { get; set; }
        public string ResolucionHabilitacion { get; set; }
        public DateTime FechaHabilitacion { get; set; }
        public string SoftwareId { get; set; }
        public string PinSoftware { get; set; }
    }

    public class ActualizarEmisorDTO
    {
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public DireccionDTO Direccion { get; set; }
        public ContactoDTO Contacto { get; set; }
        public string RegistroMercantil { get; set; }
    }

    public class ActualizarCertificadoDTO
    {
        public string CertificadoDigital { get; set; }
        public string ClaveCertificado { get; set; }
    }
}
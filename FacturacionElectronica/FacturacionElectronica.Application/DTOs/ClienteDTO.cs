using FacturacionElectronica.Domain.Enums;

namespace FacturacionElectronica.Application.DTOs
{
    public class ClienteDTO
    {
        public string Identificacion { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public TipoPersona TipoPersona { get; set; }
        public TipoResponsabilidadFiscal ResponsabilidadFiscal { get; set; }
        public DireccionDTO Direccion { get; set; }
        public ContactoDTO Contacto { get; set; }
        public string RegistroMercantil { get; set; }
        public string CodigoPostal { get; set; }
        public string CodigoCiudad { get; set; }
        public string CodigoDepartamento { get; set; }
        public string CodigoPais { get; set; }
    }

    public class CrearClienteDTO
    {
        public string Identificacion { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public TipoPersona TipoPersona { get; set; }
        public TipoResponsabilidadFiscal ResponsabilidadFiscal { get; set; }
        public DireccionDTO Direccion { get; set; }
        public ContactoDTO Contacto { get; set; }
        public string RegistroMercantil { get; set; }
    }

    public class ActualizarClienteDTO
    {
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public TipoResponsabilidadFiscal ResponsabilidadFiscal { get; set; }
        public DireccionDTO Direccion { get; set; }
        public ContactoDTO Contacto { get; set; }
        public string RegistroMercantil { get; set; }
        public string CodigoPostal { get; set; }
        public string CodigoCiudad { get; set; }
        public string CodigoDepartamento { get; set; }
        public string CodigoPais { get; set; }
    }
}
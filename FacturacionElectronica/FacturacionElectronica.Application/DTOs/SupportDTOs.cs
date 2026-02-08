using FacturacionElectronica.Domain.Enums;

namespace FacturacionElectronica.Application.DTOs
{
    public class DireccionDTO
    {
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Ciudad { get; set; }
        public string Departamento { get; set; }
        public string CodigoPostal { get; set; }
        public string Pais { get; set; }
    }

    public class ContactoDTO
    {
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string PaginaWeb { get; set; }
    }

    public class ValorMonetarioDTO
    {
        public decimal Valor { get; set; }
        public string Moneda { get; set; } = "COP";
    }

    public class NumeracionDocumentoDTO
    {
        public Guid Id { get; set; }
        public string Prefijo { get; set; }
        public long NumeroInicial { get; set; }
        public long NumeroFinal { get; set; }
        public long NumeroActual { get; set; }
        public DateTime FechaAutorizacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public string ResolucionAutorizacion { get; set; }
        public bool Activo { get; set; }
    }

    public class CrearNumeracionDocumentoDTO
    {
        public string Prefijo { get; set; }
        public long NumeroInicial { get; set; }
        public long NumeroFinal { get; set; }
        public DateTime FechaAutorizacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public string ResolucionAutorizacion { get; set; }
    }

    public class ItemDocumentoDTO
    {
        public Guid Id { get; set; }
        public int Orden { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string CodigoEstandar { get; set; }
        public decimal Cantidad { get; set; }
        public UnidadMedida UnidadMedida { get; set; }
        public ValorMonetarioDTO ValorUnitario { get; set; }
        public ValorMonetarioDTO ValorTotal { get; set; }
        public ValorMonetarioDTO? Descuento { get; set; }
        public string Observaciones { get; set; }
    }

    public class CrearItemDocumentoDTO
    {
        public int Orden { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string CodigoEstandar { get; set; }
        public decimal Cantidad { get; set; }
        public UnidadMedida UnidadMedida { get; set; }
        public ValorMonetarioDTO ValorUnitario { get; set; }
        public ValorMonetarioDTO? Descuento { get; set; }
        public string Observaciones { get; set; }
    }

    public class ImpuestoDocumentoDTO
    {
        public Guid Id { get; set; }
        public TipoImpuesto TipoImpuesto { get; set; }
        public decimal Porcentaje { get; set; }
        public ValorMonetarioDTO BaseGravable { get; set; }
        public ValorMonetarioDTO Valor { get; set; }
        public string CodigoTributo { get; set; }
        public string NombreTributo { get; set; }
    }

    public class CrearImpuestoDocumentoDTO
    {
        public TipoImpuesto TipoImpuesto { get; set; }
        public decimal Porcentaje { get; set; }
        public ValorMonetarioDTO BaseGravable { get; set; }
        public string CodigoTributo { get; set; }
        public string NombreTributo { get; set; }
    }

    public class PagoDocumentoDTO
    {
        public Guid Id { get; set; }
        public string MetodoPago { get; set; }
        public ValorMonetarioDTO Valor { get; set; }
        public DateTime FechaPago { get; set; }
        public string Referencia { get; set; }
        public string Observaciones { get; set; }
    }

    public class CrearPagoDocumentoDTO
    {
        public string MetodoPago { get; set; }
        public ValorMonetarioDTO Valor { get; set; }
        public DateTime FechaPago { get; set; }
        public string Referencia { get; set; }
        public string Observaciones { get; set; }
    }

    public class EventoDocumentoDTO
    {
        public Guid Id { get; set; }
        public string TipoEvento { get; set; }
        public string Descripcion { get; set; }
        public string Detalles { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
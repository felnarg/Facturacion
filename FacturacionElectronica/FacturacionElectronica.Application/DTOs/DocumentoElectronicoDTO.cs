using FacturacionElectronica.Domain.Enums;

namespace FacturacionElectronica.Application.DTOs
{
    public class DocumentoElectronicoDTO
    {
        public Guid Id { get; set; }
        public string NumeroDocumento { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public EstadoDocumento Estado { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string Observaciones { get; set; }
        public string Cufe { get; set; }
        public string QrCode { get; set; }
        public string RespuestaDian { get; set; }
        public DateTime? FechaTransmision { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        
        public ValorMonetarioDTO Subtotal { get; set; }
        public ValorMonetarioDTO TotalDescuentos { get; set; }
        public ValorMonetarioDTO TotalImpuestos { get; set; }
        public ValorMonetarioDTO Total { get; set; }
        public ValorMonetarioDTO TotalPagado { get; set; }
        public ValorMonetarioDTO SaldoPendiente { get; set; }
        
        public string EmisorId { get; set; }
        public string ClienteId { get; set; }
        
        public List<ItemDocumentoDTO> Items { get; set; } = new();
        public List<ImpuestoDocumentoDTO> Impuestos { get; set; } = new();
        public List<PagoDocumentoDTO> Pagos { get; set; } = new();
        public List<EventoDocumentoDTO> Eventos { get; set; } = new();
    }

    public class CrearDocumentoElectronicoDTO
    {
        public TipoDocumento TipoDocumento { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Observaciones { get; set; }
        public string ClienteId { get; set; }
        public List<CrearItemDocumentoDTO> Items { get; set; } = new();
        public List<CrearImpuestoDocumentoDTO> Impuestos { get; set; } = new();
    }

    public class GenerarDocumentoDTO
    {
        public Guid DocumentoId { get; set; }
    }

    public class FirmarDocumentoDTO
    {
        public Guid DocumentoId { get; set; }
    }

    public class TransmitirDocumentoDTO
    {
        public Guid DocumentoId { get; set; }
    }
}
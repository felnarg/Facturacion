using FacturacionElectronica.Domain.Entities;

namespace FacturacionElectronica.Domain.Events
{
    public class DocumentoGeneradoEvent : IDomainEvent
    {
        public Guid DocumentoId { get; }
        public string NumeroDocumento { get; }
        public string EmisorId { get; }
        public string ClienteId { get; }
        public decimal Total { get; }
        public DateTime FechaEmision { get; }

        public DocumentoGeneradoEvent(DocumentoElectronico documento)
        {
            DocumentoId = documento.Id;
            NumeroDocumento = documento.NumeroDocumento;
            EmisorId = documento.EmisorId;
            ClienteId = documento.ClienteId;
            Total = documento.Total.Valor;
            FechaEmision = documento.FechaEmision;
        }
    }

    public class DocumentoFirmadoEvent : IDomainEvent
    {
        public Guid DocumentoId { get; }
        public string NumeroDocumento { get; }
        public string Cufe { get; }
        public DateTime FechaFirma { get; }

        public DocumentoFirmadoEvent(DocumentoElectronico documento)
        {
            DocumentoId = documento.Id;
            NumeroDocumento = documento.NumeroDocumento;
            Cufe = documento.Cufe;
            FechaFirma = DateTime.UtcNow;
        }
    }

    public class DocumentoTransmitidoEvent : IDomainEvent
    {
        public Guid DocumentoId { get; }
        public string NumeroDocumento { get; }
        public DateTime FechaTransmision { get; }

        public DocumentoTransmitidoEvent(DocumentoElectronico documento)
        {
            DocumentoId = documento.Id;
            NumeroDocumento = documento.NumeroDocumento;
            FechaTransmision = documento.FechaTransmision.Value;
        }
    }

    public class DocumentoAceptadoEvent : IDomainEvent
    {
        public Guid DocumentoId { get; }
        public string NumeroDocumento { get; }
        public string RespuestaDian { get; }
        public DateTime FechaRespuesta { get; }

        public DocumentoAceptadoEvent(DocumentoElectronico documento)
        {
            DocumentoId = documento.Id;
            NumeroDocumento = documento.NumeroDocumento;
            RespuestaDian = documento.RespuestaDian;
            FechaRespuesta = documento.FechaRespuesta.Value;
        }
    }

    public class DocumentoRechazadoEvent : IDomainEvent
    {
        public Guid DocumentoId { get; }
        public string NumeroDocumento { get; }
        public string RespuestaDian { get; }
        public DateTime FechaRespuesta { get; }

        public DocumentoRechazadoEvent(DocumentoElectronico documento)
        {
            DocumentoId = documento.Id;
            NumeroDocumento = documento.NumeroDocumento;
            RespuestaDian = documento.RespuestaDian;
            FechaRespuesta = documento.FechaRespuesta.Value;
        }
    }

    public interface IDomainEvent
    {
    }
}
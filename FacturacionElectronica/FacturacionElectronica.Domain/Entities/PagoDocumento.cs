using FacturacionElectronica.Domain.ValueObjects;

namespace FacturacionElectronica.Domain.Entities
{
    public class PagoDocumento : BaseEntity
    {
        public string MetodoPago { get; private set; }
        public ValorMonetario Valor { get; private set; }
        public DateTime FechaPago { get; private set; }
        public string Referencia { get; private set; }
        public string Observaciones { get; private set; }
        
        // Foreign Key
        public Guid DocumentoId { get; private set; }
        public DocumentoElectronico Documento { get; private set; }

        private PagoDocumento() { }

        public PagoDocumento(
            string metodoPago,
            ValorMonetario valor,
            DateTime fechaPago,
            string referencia,
            Guid documentoId)
        {
            if (string.IsNullOrWhiteSpace(metodoPago))
                throw new ArgumentException("El método de pago es requerido");
            if (valor.Valor <= 0)
                throw new ArgumentException("El valor del pago debe ser mayor a 0");
            if (string.IsNullOrWhiteSpace(referencia))
                throw new ArgumentException("La referencia es requerida");

            MetodoPago = metodoPago;
            Valor = valor;
            FechaPago = fechaPago;
            Referencia = referencia;
            DocumentoId = documentoId;
        }

        public void ActualizarObservaciones(string observaciones)
        {
            Observaciones = observaciones;
            ActualizarFechaModificacion();
        }

        public void ActualizarReferencia(string referencia)
        {
            Referencia = referencia;
            ActualizarFechaModificacion();
        }
    }
}
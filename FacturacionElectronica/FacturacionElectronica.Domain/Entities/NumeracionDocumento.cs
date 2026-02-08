using FacturacionElectronica.Domain.Enums;

namespace FacturacionElectronica.Domain.Entities
{
    public class NumeracionDocumento : BaseEntity
    {
        public string Prefijo { get; private set; }
        public long NumeroInicial { get; private set; }
        public long NumeroFinal { get; private set; }
        public long NumeroActual { get; private set; }
        public DateTime FechaAutorizacion { get; private set; }
        public DateTime FechaVencimiento { get; private set; }
        public TipoDocumento TipoDocumento { get; private set; }
        public string ResolucionAutorizacion { get; private set; }
        
        // Foreign Key
        public string EmisorId { get; private set; }
        public Emisor Emisor { get; private set; }

        private NumeracionDocumento() { }

        public NumeracionDocumento(
            string prefijo,
            long numeroInicial,
            long numeroFinal,
            DateTime fechaAutorizacion,
            DateTime fechaVencimiento,
            TipoDocumento tipoDocumento,
            string resolucionAutorizacion,
            string emisorId)
        {
            if (string.IsNullOrWhiteSpace(prefijo))
                throw new ArgumentException("El prefijo es requerido");
            if (numeroInicial <= 0)
                throw new ArgumentException("El número inicial debe ser mayor a 0");
            if (numeroFinal <= numeroInicial)
                throw new ArgumentException("El número final debe ser mayor al número inicial");
            if (fechaVencimiento <= fechaAutorizacion)
                throw new ArgumentException("La fecha de vencimiento debe ser posterior a la fecha de autorización");
            if (string.IsNullOrWhiteSpace(resolucionAutorizacion))
                throw new ArgumentException("La resolución de autorización es requerida");
            if (string.IsNullOrWhiteSpace(emisorId))
                throw new ArgumentException("El ID del emisor es requerido");

            Prefijo = prefijo;
            NumeroInicial = numeroInicial;
            NumeroFinal = numeroFinal;
            NumeroActual = numeroInicial;
            FechaAutorizacion = fechaAutorizacion;
            FechaVencimiento = fechaVencimiento;
            TipoDocumento = tipoDocumento;
            ResolucionAutorizacion = resolucionAutorizacion;
            EmisorId = emisorId;
        }

        public string ObtenerSiguienteNumero()
        {
            if (NumeroActual > NumeroFinal)
                throw new InvalidOperationException("Se ha alcanzado el límite de numeración autorizada");

            var siguienteNumero = NumeroActual;
            NumeroActual++;
            ActualizarFechaModificacion();

            return $"{Prefijo}{siguienteNumero:D19}";
        }

        public bool EstaVencida()
        {
            return DateTime.UtcNow > FechaVencimiento;
        }

        public bool EstaDisponible()
        {
            return Activo && !EstaVencida() && NumeroActual <= NumeroFinal;
        }

        public void Renovar(DateTime nuevaFechaVencimiento, string nuevaResolucion)
        {
            if (nuevaFechaVencimiento <= FechaVencimiento)
                throw new ArgumentException("La nueva fecha de vencimiento debe ser posterior a la actual");

            FechaVencimiento = nuevaFechaVencimiento;
            ResolucionAutorizacion = nuevaResolucion;
            ActualizarFechaModificacion();
        }
    }
}
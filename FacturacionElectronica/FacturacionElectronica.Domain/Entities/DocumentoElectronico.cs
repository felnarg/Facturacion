using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.ValueObjects;

namespace FacturacionElectronica.Domain.Entities
{
    public class DocumentoElectronico : BaseEntity
    {
        public string NumeroDocumento { get; private set; }
        public TipoDocumento TipoDocumento { get; private set; }
        public EstadoDocumento Estado { get; private set; }
        public DateTime FechaEmision { get; private set; }
        public DateTime FechaVencimiento { get; private set; }
        public DateTime? FechaEntrega { get; private set; }
        public string Observaciones { get; private set; }
        public string Cufe { get; private set; }
        public string QrCode { get; private set; }
        public string XmlContent { get; private set; }
        public string XmlFirmado { get; private set; }
        public string RespuestaDian { get; private set; }
        public DateTime? FechaTransmision { get; private set; }
        public DateTime? FechaRespuesta { get; private set; }
        
        // Valores monetarios
        public ValorMonetario Subtotal { get; private set; }
        public ValorMonetario TotalDescuentos { get; private set; }
        public ValorMonetario TotalImpuestos { get; private set; }
        public ValorMonetario Total { get; private set; }
        public ValorMonetario TotalPagado { get; private set; }
        public ValorMonetario SaldoPendiente { get; private set; }
        
        // Foreign Keys
        public string EmisorId { get; private set; }
        public Emisor Emisor { get; private set; }
        
        public string ClienteId { get; private set; }
        public Cliente Cliente { get; private set; }
        
        // Navegación
        private readonly List<ItemDocumento> _items = new();
        public IReadOnlyCollection<ItemDocumento> Items => _items.AsReadOnly();
        
        private readonly List<ImpuestoDocumento> _impuestos = new();
        public IReadOnlyCollection<ImpuestoDocumento> Impuestos => _impuestos.AsReadOnly();
        
        private readonly List<PagoDocumento> _pagos = new();
        public IReadOnlyCollection<PagoDocumento> Pagos => _pagos.AsReadOnly();
        
        private readonly List<EventoDocumento> _eventos = new();
        public IReadOnlyCollection<EventoDocumento> Eventos => _eventos.AsReadOnly();

        private DocumentoElectronico() { }

        public DocumentoElectronico(
            string numeroDocumento,
            TipoDocumento tipoDocumento,
            DateTime fechaEmision,
            DateTime fechaVencimiento,
            string emisorId,
            string clienteId)
        {
            if (string.IsNullOrWhiteSpace(numeroDocumento))
                throw new ArgumentException("El número de documento es requerido");
            if (string.IsNullOrWhiteSpace(emisorId))
                throw new ArgumentException("El ID del emisor es requerido");
            if (string.IsNullOrWhiteSpace(clienteId))
                throw new ArgumentException("El ID del cliente es requerido");
            if (fechaVencimiento < fechaEmision)
                throw new ArgumentException("La fecha de vencimiento no puede ser anterior a la fecha de emisión");

            NumeroDocumento = numeroDocumento;
            TipoDocumento = tipoDocumento;
            Estado = EstadoDocumento.Pendiente;
            FechaEmision = fechaEmision;
            FechaVencimiento = fechaVencimiento;
            EmisorId = emisorId;
            ClienteId = clienteId;
            
            // Inicializar valores monetarios
            Subtotal = new ValorMonetario(0);
            TotalDescuentos = new ValorMonetario(0);
            TotalImpuestos = new ValorMonetario(0);
            Total = new ValorMonetario(0);
            TotalPagado = new ValorMonetario(0);
            SaldoPendiente = new ValorMonetario(0);
        }

        public void AgregarItem(ItemDocumento item)
        {
            _items.Add(item);
            RecalcularTotales();
            AgregarEvento("Item agregado", $"Se agregó el item: {item.Descripcion}");
        }

        public void AgregarImpuesto(ImpuestoDocumento impuesto)
        {
            _impuestos.Add(impuesto);
            RecalcularTotales();
            AgregarEvento("Impuesto agregado", $"Se agregó impuesto: {impuesto.TipoImpuesto}");
        }

        public void AgregarPago(PagoDocumento pago)
        {
            _pagos.Add(pago);
            RecalcularPagos();
            AgregarEvento("Pago registrado", $"Pago de {pago.Valor.Valor} {pago.Valor.Moneda}");
        }

        private void RecalcularTotales()
        {
            var subtotal = _items.Sum(i => i.ValorTotal.Valor);
            var descuentos = _items.Sum(i => i.Descuento?.Valor ?? 0);
            var impuestos = _impuestos.Sum(i => i.Valor.Valor);

            Subtotal = new ValorMonetario(subtotal);
            TotalDescuentos = new ValorMonetario(descuentos);
            TotalImpuestos = new ValorMonetario(impuestos);
            Total = new ValorMonetario(subtotal - descuentos + impuestos);
            SaldoPendiente = new ValorMonetario(Total.Valor - TotalPagado.Valor);

            ActualizarFechaModificacion();
        }

        private void RecalcularPagos()
        {
            TotalPagado = new ValorMonetario(_pagos.Sum(p => p.Valor.Valor));
            SaldoPendiente = new ValorMonetario(Total.Valor - TotalPagado.Valor);
            
            if (SaldoPendiente.Valor <= 0 && Estado == EstadoDocumento.Aceptado)
            {
                Estado = EstadoDocumento.Transmitido;
                AgregarEvento("Documento pagado", "El documento ha sido completamente pagado");
            }

            ActualizarFechaModificacion();
        }

        public void MarcarComoGenerado(string xmlContent)
        {
            if (Estado != EstadoDocumento.Pendiente)
                throw new InvalidOperationException("Solo documentos pendientes pueden ser generados");

            XmlContent = xmlContent;
            Estado = EstadoDocumento.Generado;
            AgregarEvento("Documento generado", "XML generado correctamente");
        }

        public void MarcarComoFirmado(string xmlFirmado, string cufe)
        {
            if (Estado != EstadoDocumento.Generado)
                throw new InvalidOperationException("Solo documentos generados pueden ser firmados");

            XmlFirmado = xmlFirmado;
            Cufe = cufe;
            Estado = EstadoDocumento.Firmado;
            AgregarEvento("Documento firmado", $"CUFE generado: {cufe}");
        }

        public void MarcarComoTransmitido(DateTime fechaTransmision)
        {
            if (Estado != EstadoDocumento.Firmado && Estado != EstadoDocumento.Validado)
                throw new InvalidOperationException("Solo documentos firmados o validados pueden ser transmitidos");

            FechaTransmision = fechaTransmision;
            Estado = EstadoDocumento.Transmitido;
            AgregarEvento("Documento transmitido", "Transmitido a la DIAN");
        }

        public void ProcesarRespuestaDian(string respuesta, bool aceptado, DateTime fechaRespuesta)
        {
            RespuestaDian = respuesta;
            FechaRespuesta = fechaRespuesta;
            Estado = aceptado ? EstadoDocumento.Aceptado : EstadoDocumento.Rechazado;
            
            var evento = aceptado ? "Documento aceptado" : "Documento rechazado";
            AgregarEvento(evento, respuesta);
        }

        public void MarcarComoContingencia()
        {
            Estado = EstadoDocumento.Contingencia;
            AgregarEvento("Modo contingencia", "Documento procesado en modo de contingencia");
        }

        private void AgregarEvento(string tipo, string descripcion)
        {
            _eventos.Add(new EventoDocumento(Id, tipo, descripcion));
        }

        public void ActualizarObservaciones(string observaciones)
        {
            Observaciones = observaciones;
            ActualizarFechaModificacion();
        }

        public void ActualizarFechaEntrega(DateTime fechaEntrega)
        {
            FechaEntrega = fechaEntrega;
            ActualizarFechaModificacion();
            AgregarEvento("Fecha entrega actualizada", $"Nueva fecha: {fechaEntrega:yyyy-MM-dd}");
        }

        public void GenerarQrCode(string qrContent)
        {
            QrCode = qrContent;
            ActualizarFechaModificacion();
            AgregarEvento("QR generado", "Código QR generado correctamente");
        }
    }
}
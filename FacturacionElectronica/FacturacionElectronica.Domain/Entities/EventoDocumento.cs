namespace FacturacionElectronica.Domain.Entities
{
    public class EventoDocumento : BaseEntity
    {
        public string TipoEvento { get; private set; }
        public string Descripcion { get; private set; }
        public string Detalles { get; private set; }
        public string Usuario { get; private set; }
        
        // Foreign Key
        public Guid DocumentoId { get; private set; }
        public DocumentoElectronico Documento { get; private set; }

        private EventoDocumento() { }

        public EventoDocumento(
            Guid documentoId,
            string tipoEvento,
            string descripcion,
            string usuario = "Sistema")
        {
            if (string.IsNullOrWhiteSpace(tipoEvento))
                throw new ArgumentException("El tipo de evento es requerido");
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción es requerida");

            DocumentoId = documentoId;
            TipoEvento = tipoEvento;
            Descripcion = descripcion;
            Usuario = usuario;
        }

        public void AgregarDetalles(string detalles)
        {
            Detalles = detalles;
            ActualizarFechaModificacion();
        }

        public void ActualizarUsuario(string usuario)
        {
            Usuario = usuario;
            ActualizarFechaModificacion();
        }
    }
}
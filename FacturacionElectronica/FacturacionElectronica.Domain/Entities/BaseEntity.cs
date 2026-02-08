namespace FacturacionElectronica.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime FechaCreacion { get; protected set; }
        public DateTime? FechaModificacion { get; protected set; }
        public bool Activo { get; protected set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            FechaCreacion = DateTime.UtcNow;
            Activo = true;
        }

        public void Desactivar()
        {
            Activo = false;
            FechaModificacion = DateTime.UtcNow;
        }

        public void Activar()
        {
            Activo = true;
            FechaModificacion = DateTime.UtcNow;
        }

        protected void ActualizarFechaModificacion()
        {
            FechaModificacion = DateTime.UtcNow;
        }
    }

    public abstract class Entity<TId> : BaseEntity
    {
        public TId Codigo { get; protected set; }

        protected Entity(TId codigo)
        {
            Codigo = codigo;
        }
    }
}
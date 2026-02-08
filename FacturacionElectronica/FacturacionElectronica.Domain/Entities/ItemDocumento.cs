using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.ValueObjects;

namespace FacturacionElectronica.Domain.Entities
{
    public class ItemDocumento : BaseEntity
    {
        public int Orden { get; private set; }
        public string Codigo { get; private set; }
        public string Descripcion { get; private set; }
        public string CodigoEstandar { get; private set; } // CCE, GTIN, etc.
        public decimal Cantidad { get; private set; }
        public UnidadMedida UnidadMedida { get; private set; }
        public ValorMonetario ValorUnitario { get; private set; }
        public ValorMonetario ValorTotal { get; private set; }
        public ValorMonetario? Descuento { get; private set; }
        public string Observaciones { get; private set; }
        
        // Foreign Key
        public Guid DocumentoId { get; private set; }
        public DocumentoElectronico Documento { get; private set; }

        private ItemDocumento() { }

        public ItemDocumento(
            int orden,
            string codigo,
            string descripcion,
            decimal cantidad,
            UnidadMedida unidadMedida,
            ValorMonetario valorUnitario,
            Guid documentoId)
        {
            if (orden <= 0)
                throw new ArgumentException("El orden debe ser mayor a 0");
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código es requerido");
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción es requerida");
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0");

            Orden = orden;
            Codigo = codigo;
            Descripcion = descripcion;
            Cantidad = cantidad;
            UnidadMedida = unidadMedida;
            ValorUnitario = valorUnitario;
            DocumentoId = documentoId;
            
            CalcularValores();
        }

        private void CalcularValores()
        {
            var valorTotal = Cantidad * ValorUnitario.Valor;
            ValorTotal = new ValorMonetario(valorTotal, ValorUnitario.Moneda);
            
            if (Descuento.HasValue)
            {
                ValorTotal = ValorTotal.Sumar(new ValorMonetario(-Descuento.Value.Valor, Descuento.Value.Moneda));
            }
        }

        public void AplicarDescuento(ValorMonetario descuento)
        {
            if (descuento.Valor > ValorTotal.Valor)
                throw new ArgumentException("El descuento no puede ser mayor al valor total");

            Descuento = descuento;
            CalcularValores();
            ActualizarFechaModificacion();
        }

        public void ActualizarCantidad(decimal nuevaCantidad)
        {
            if (nuevaCantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0");

            Cantidad = nuevaCantidad;
            CalcularValores();
            ActualizarFechaModificacion();
        }

        public void ActualizarValorUnitario(ValorMonetario nuevoValor)
        {
            ValorUnitario = nuevoValor;
            CalcularValores();
            ActualizarFechaModificacion();
        }

        public void ActualizarCodigoEstandar(string codigoEstandar)
        {
            CodigoEstandar = codigoEstandar;
            ActualizarFechaModificacion();
        }

        public void ActualizarObservaciones(string observaciones)
        {
            Observaciones = observaciones;
            ActualizarFechaModificacion();
        }
    }
}
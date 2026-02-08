using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.ValueObjects;

namespace FacturacionElectronica.Domain.Entities
{
    public class ImpuestoDocumento : BaseEntity
    {
        public TipoImpuesto TipoImpuesto { get; private set; }
        public decimal Porcentaje { get; private set; }
        public ValorMonetario BaseGravable { get; private set; }
        public ValorMonetario Valor { get; private set; }
        public string CodigoTributo { get; private set; }
        public string NombreTributo { get; private set; }
        
        // Foreign Key
        public Guid DocumentoId { get; private set; }
        public DocumentoElectronico Documento { get; private set; }

        private ImpuestoDocumento() { }

        public ImpuestoDocumento(
            TipoImpuesto tipoImpuesto,
            decimal porcentaje,
            ValorMonetario baseGravable,
            string codigoTributo,
            string nombreTributo,
            Guid documentoId)
        {
            if (porcentaje < 0 || porcentaje > 100)
                throw new ArgumentException("El porcentaje debe estar entre 0 y 100");
            if (string.IsNullOrWhiteSpace(codigoTributo))
                throw new ArgumentException("El código del tributo es requerido");
            if (string.IsNullOrWhiteSpace(nombreTributo))
                throw new ArgumentException("El nombre del tributo es requerido");

            TipoImpuesto = tipoImpuesto;
            Porcentaje = porcentaje;
            BaseGravable = baseGravable;
            CodigoTributo = codigoTributo;
            NombreTributo = nombreTributo;
            DocumentoId = documentoId;
            
            CalcularValor();
        }

        private void CalcularValor()
        {
            var valor = BaseGravable.Valor * (Porcentaje / 100);
            Valor = new ValorMonetario(valor, BaseGravable.Moneda);
        }

        public void ActualizarPorcentaje(decimal nuevoPorcentaje)
        {
            if (nuevoPorcentaje < 0 || nuevoPorcentaje > 100)
                throw new ArgumentException("El porcentaje debe estar entre 0 y 100");

            Porcentaje = nuevoPorcentaje;
            CalcularValor();
            ActualizarFechaModificacion();
        }

        public void ActualizarBaseGravable(ValorMonetario nuevaBase)
        {
            BaseGravable = nuevaBase;
            CalcularValor();
            ActualizarFechaModificacion();
        }

        public void ActualizarInformacionTributo(string codigoTributo, string nombreTributo)
        {
            CodigoTributo = codigoTributo;
            NombreTributo = nombreTributo;
            ActualizarFechaModificacion();
        }
    }
}
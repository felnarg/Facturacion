using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.ValueObjects;

namespace FacturacionElectronica.Domain.Entities
{
    public class Cliente : Entity<string>
    {
        public string RazonSocial { get; private set; }
        public string NombreComercial { get; private set; }
        public TipoPersona TipoPersona { get; private set; }
        public TipoResponsabilidadFiscal ResponsabilidadFiscal { get; private set; }
        public Direccion Direccion { get; private set; }
        public InformacionContacto Contacto { get; private set; }
        public string RegistroMercantil { get; private set; }
        public string CodigoPostal { get; private set; }
        public string CodigoCiudad { get; private set; }
        public string CodigoDepartamento { get; private set; }
        public string CodigoPais { get; private set; }

        // Navegación
        private readonly List<DocumentoElectronico> _documentos = new();
        public IReadOnlyCollection<DocumentoElectronico> Documentos => _documentos.AsReadOnly();

        private Cliente() : base(default!) { }

        public Cliente(
            string identificacion,
            string razonSocial,
            TipoPersona tipoPersona,
            TipoResponsabilidadFiscal responsabilidadFiscal,
            Direccion direccion,
            InformacionContacto contacto) : base(identificacion)
        {
            if (string.IsNullOrWhiteSpace(identificacion))
                throw new ArgumentException("La identificación es requerida");
            if (string.IsNullOrWhiteSpace(razonSocial))
                throw new ArgumentException("La razón social es requerida");

            RazonSocial = razonSocial;
            TipoPersona = tipoPersona;
            ResponsabilidadFiscal = responsabilidadFiscal;
            Direccion = direccion ?? throw new ArgumentNullException(nameof(direccion));
            Contacto = contacto ?? throw new ArgumentNullException(nameof(contacto));

            // Valores por defecto para Colombia
            CodigoPais = "CO";
            CodigoDepartamento = "11"; // Bogotá por defecto
            CodigoCiudad = "11001"; // Bogotá por defecto

            ActualizarFechaModificacion();
        }

        public void ActualizarInformacion(
            string razonSocial,
            string nombreComercial,
            TipoResponsabilidadFiscal responsabilidadFiscal,
            Direccion direccion,
            InformacionContacto contacto,
            string registroMercantil)
        {
            RazonSocial = razonSocial;
            NombreComercial = nombreComercial;
            ResponsabilidadFiscal = responsabilidadFiscal;
            Direccion = direccion;
            Contacto = contacto;
            RegistroMercantil = registroMercantil;

            ActualizarFechaModificacion();
        }

        public void ActualizarUbicacion(
            string codigoPostal,
            string codigoCiudad,
            string codigoDepartamento,
            string codigoPais)
        {
            CodigoPostal = codigoPostal;
            CodigoCiudad = codigoCiudad;
            CodigoDepartamento = codigoDepartamento;
            CodigoPais = codigoPais;

            ActualizarFechaModificacion();
        }

        public bool EsConsumidorFinal()
        {
            return Codigo == "222222222222" && RazonSocial.ToUpper() == "CONSUMIDOR FINAL";
        }
    }
}
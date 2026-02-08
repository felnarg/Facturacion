using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.ValueObjects;

namespace FacturacionElectronica.Domain.Entities
{
    public class Emisor : Entity<string>
    {
        public string RazonSocial { get; private set; }
        public string NombreComercial { get; private set; }
        public TipoPersona TipoPersona { get; private set; }
        public TipoResponsabilidadFiscal ResponsabilidadFiscal { get; private set; }
        public Direccion Direccion { get; private set; }
        public InformacionContacto Contacto { get; private set; }
        public string RegistroMercantil { get; private set; }
        public string ResolucionHabilitacion { get; private set; }
        public DateTime FechaHabilitacion { get; private set; }
        public string SoftwareId { get; private set; }
        public string PinSoftware { get; private set; }
        public string CertificadoDigital { get; private set; }
        public string ClaveCertificado { get; private set; }

        // Navegación
        private readonly List<NumeracionDocumento> _numeraciones = new();
        public IReadOnlyCollection<NumeracionDocumento> Numeraciones => _numeraciones.AsReadOnly();

        private Emisor() : base(default!) { }

        public Emisor(
            string nit,
            string razonSocial,
            TipoPersona tipoPersona,
            TipoResponsabilidadFiscal responsabilidadFiscal,
            Direccion direccion,
            InformacionContacto contacto,
            string resolucionHabilitacion,
            DateTime fechaHabilitacion,
            string softwareId,
            string pinSoftware) : base(nit)
        {
            if (string.IsNullOrWhiteSpace(nit))
                throw new ArgumentException("El NIT es requerido");
            if (string.IsNullOrWhiteSpace(razonSocial))
                throw new ArgumentException("La razón social es requerida");
            if (string.IsNullOrWhiteSpace(resolucionHabilitacion))
                throw new ArgumentException("La resolución de habilitación es requerida");
            if (string.IsNullOrWhiteSpace(softwareId))
                throw new ArgumentException("El Software ID es requerido");
            if (string.IsNullOrWhiteSpace(pinSoftware))
                throw new ArgumentException("El PIN del software es requerido");

            RazonSocial = razonSocial;
            TipoPersona = tipoPersona;
            ResponsabilidadFiscal = responsabilidadFiscal;
            Direccion = direccion ?? throw new ArgumentNullException(nameof(direccion));
            Contacto = contacto ?? throw new ArgumentNullException(nameof(contacto));
            ResolucionHabilitacion = resolucionHabilitacion;
            FechaHabilitacion = fechaHabilitacion;
            SoftwareId = softwareId;
            PinSoftware = pinSoftware;

            ActualizarFechaModificacion();
        }

        public void ActualizarInformacion(
            string razonSocial,
            string nombreComercial,
            Direccion direccion,
            InformacionContacto contacto,
            string registroMercantil)
        {
            RazonSocial = razonSocial;
            NombreComercial = nombreComercial;
            Direccion = direccion;
            Contacto = contacto;
            RegistroMercantil = registroMercantil;

            ActualizarFechaModificacion();
        }

        public void ActualizarCertificado(string certificadoDigital, string claveCertificado)
        {
            CertificadoDigital = certificadoDigital;
            ClaveCertificado = claveCertificado;

            ActualizarFechaModificacion();
        }

        public void AgregarNumeracion(NumeracionDocumento numeracion)
        {
            if (_numeraciones.Any(n => n.TipoDocumento == numeracion.TipoDocumento && n.Activo))
                throw new InvalidOperationException($"Ya existe una numeración activa para el tipo de documento {numeracion.TipoDocumento}");

            _numeraciones.Add(numeracion);
            ActualizarFechaModificacion();
        }

        public NumeracionDocumento ObtenerNumeracionActiva(TipoDocumento tipoDocumento)
        {
            return _numeraciones.FirstOrDefault(n => n.TipoDocumento == tipoDocumento && n.Activo)
                ?? throw new InvalidOperationException($"No existe numeración activa para el tipo de documento {tipoDocumento}");
        }
    }
}
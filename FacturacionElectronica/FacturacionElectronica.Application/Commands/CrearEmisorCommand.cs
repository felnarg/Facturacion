using FacturacionElectronica.Application.DTOs;
using MediatR;

namespace FacturacionElectronica.Application.Commands
{
    public class CrearEmisorCommand : IRequest<EmisorDTO>
    {
        public CrearEmisorDTO Emisor { get; }

        public CrearEmisorCommand(CrearEmisorDTO emisor)
        {
            Emisor = emisor;
        }
    }

    public class ActualizarEmisorCommand : IRequest<bool>
    {
        public string Nit { get; }
        public ActualizarEmisorDTO Emisor { get; }

        public ActualizarEmisorCommand(string nit, ActualizarEmisorDTO emisor)
        {
            Nit = nit;
            Emisor = emisor;
        }
    }

    public class ActualizarCertificadoEmisorCommand : IRequest<bool>
    {
        public string Nit { get; }
        public ActualizarCertificadoDTO Certificado { get; }

        public ActualizarCertificadoEmisorCommand(string nit, ActualizarCertificadoDTO certificado)
        {
            Nit = nit;
            Certificado = certificado;
        }
    }

    public class AgregarNumeracionCommand : IRequest<NumeracionDocumentoDTO>
    {
        public string EmisorId { get; }
        public CrearNumeracionDocumentoDTO Numeracion { get; }

        public AgregarNumeracionCommand(string emisorId, CrearNumeracionDocumentoDTO numeracion)
        {
            EmisorId = emisorId;
            Numeracion = numeracion;
        }
    }
}
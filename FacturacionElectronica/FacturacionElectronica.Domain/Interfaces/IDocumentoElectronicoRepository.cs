using FacturacionElectronica.Domain.Entities;
using FacturacionElectronica.Domain.Enums;

namespace FacturacionElectronica.Domain.Interfaces
{
    public interface IDocumentoElectronicoRepository : IRepository<DocumentoElectronico>
    {
        Task<DocumentoElectronico> GetByNumeroAsync(string numeroDocumento);
        Task<IEnumerable<DocumentoElectronico>> GetByEmisorAsync(string emisorId, DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<IEnumerable<DocumentoElectronico>> GetByClienteAsync(string clienteId, DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<IEnumerable<DocumentoElectronico>> GetByEstadoAsync(EstadoDocumento estado, DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<IEnumerable<DocumentoElectronico>> GetByTipoAsync(TipoDocumento tipo, DateTime? fechaInicio = null, DateTime? fechaFin = null);
        Task<DocumentoElectronico> GetWithItemsAsync(Guid id);
        Task<DocumentoElectronico> GetWithAllAsync(Guid id);
        Task<bool> ExisteNumeroDocumentoAsync(string numeroDocumento);
        Task<IEnumerable<DocumentoElectronico>> GetPendientesTransmisionAsync();
        Task<IEnumerable<DocumentoElectronico>> GetVencidosAsync();
    }

    public interface IEmisorRepository : IRepository<Emisor>
    {
        Task<Emisor> GetByNitAsync(string nit);
        Task<Emisor> GetWithNumeracionesAsync(string nit);
        Task<bool> ExisteNitAsync(string nit);
    }

    public interface IClienteRepository : IRepository<Cliente>
    {
        Task<Cliente> GetByIdentificacionAsync(string identificacion);
        Task<IEnumerable<Cliente>> SearchByRazonSocialAsync(string razonSocial);
        Task<bool> ExisteIdentificacionAsync(string identificacion);
    }

    public interface INumeracionDocumentoRepository : IRepository<NumeracionDocumento>
    {
        Task<NumeracionDocumento> GetByEmisorAndTipoAsync(string emisorId, TipoDocumento tipoDocumento);
        Task<IEnumerable<NumeracionDocumento>> GetByEmisorAsync(string emisorId);
        Task<NumeracionDocumento> GetActivaByEmisorAndTipoAsync(string emisorId, TipoDocumento tipoDocumento);
        Task<bool> ExisteNumeracionAsync(string prefijo, long numeroInicial, long numeroFinal, string emisorId);
    }
}
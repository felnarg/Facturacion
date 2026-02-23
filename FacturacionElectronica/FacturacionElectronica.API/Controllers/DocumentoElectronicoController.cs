using FacturacionElectronica.Application.DTOs;
using FacturacionElectronica.Domain.Entities;
using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.Interfaces;
using FacturacionElectronica.Domain.ValueObjects;
using FacturacionElectronica.Infrastructure.EventBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FacturacionElectronica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentoElectronicoController : ControllerBase
{
    private readonly IDocumentoElectronicoRepository _documentoRepository;
    private readonly IEmisorRepository _emisorRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly INumeracionDocumentoRepository _numeracionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IXmlGeneratorService _xmlGenerator;
    private readonly IFirmaDigitalService _firmaDigital;
    private readonly IDianSoapService _dianSoap;
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    private readonly ILogger<DocumentoElectronicoController> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public DocumentoElectronicoController(
        IDocumentoElectronicoRepository documentoRepository,
        IEmisorRepository emisorRepository,
        IClienteRepository clienteRepository,
        INumeracionDocumentoRepository numeracionRepository,
        IUnitOfWork unitOfWork,
        IXmlGeneratorService xmlGenerator,
        IFirmaDigitalService firmaDigital,
        IDianSoapService dianSoap,
        IConfiguration configuration,
        IEventBus eventBus,
        ILogger<DocumentoElectronicoController> logger,
        IServiceScopeFactory scopeFactory)
    {
        _documentoRepository = documentoRepository;
        _emisorRepository    = emisorRepository;
        _clienteRepository   = clienteRepository;
        _numeracionRepository= numeracionRepository;
        _unitOfWork          = unitOfWork;
        _xmlGenerator        = xmlGenerator;
        _firmaDigital        = firmaDigital;
        _dianSoap            = dianSoap;
        _configuration       = configuration;
        _eventBus            = eventBus;
        _logger              = logger;
        _scopeFactory        = scopeFactory;
    }

    // ── GET /api/documentoselectronicos/health ────────────────────────────────────
    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "ok", timestamp = DateTime.UtcNow });

    // ── POST /api/documentoselectronicos ──────────────────────────────────────────
    /// <summary>
    /// Crea un documento electrónico, genera XML UBL 2.1, firma digitalmente,
    /// transmite a la DIAN y publica evento para Notificaciones.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CrearDocumento([FromBody] CrearDocumentoElectronicoDTO dto)
    {
        try
        {
            // ── 1. Obtener entidades ──────────────────────────────────────────────
            var emisor = await _emisorRepository.GetByNitAsync("9001234567");
            if (emisor == null)
                return NotFound("No se encontró el emisor. Asegúrate de que el seed se haya ejecutado.");

            var cliente = await _clienteRepository.GetByIdentificacionAsync(dto.ClienteId);
            if (cliente == null)
                return NotFound($"No se encontró el cliente con identificación: {dto.ClienteId}");

            var numeracion = await _numeracionRepository.GetActivaByEmisorAndTipoAsync(
                emisor.Codigo, dto.TipoDocumento);
            if (numeracion == null)
                return NotFound($"No se encontró numeración activa para: {dto.TipoDocumento}");

            // ── 2. Crear documento ────────────────────────────────────────────────
            var numeroDocumento = numeracion.ObtenerSiguienteNumero();
            var documento = new DocumentoElectronico(
                numeroDocumento: numeroDocumento,
                tipoDocumento:   dto.TipoDocumento,
                fechaEmision:    dto.FechaEmision,
                fechaVencimiento:dto.FechaVencimiento,
                emisorId:        emisor.Codigo,
                clienteId:       cliente.Codigo);

            if (!string.IsNullOrWhiteSpace(dto.Observaciones))
                documento.ActualizarObservaciones(dto.Observaciones);

            // Items
            int orden = 1;
            foreach (var itemDto in dto.Items)
            {
                var item = new ItemDocumento(
                    orden: orden++,
                    codigo: itemDto.Codigo,
                    descripcion: itemDto.Descripcion,
                    cantidad: itemDto.Cantidad,
                    unidadMedida: itemDto.UnidadMedida,
                    valorUnitario: new ValorMonetario(itemDto.ValorUnitario.Valor, itemDto.ValorUnitario.Moneda),
                    documentoId: documento.Id);

                if (!string.IsNullOrWhiteSpace(itemDto.CodigoEstandar)) item.ActualizarCodigoEstandar(itemDto.CodigoEstandar);
                if (itemDto.Descuento != null) item.AplicarDescuento(new ValorMonetario(itemDto.Descuento.Valor, itemDto.Descuento.Moneda));
                if (!string.IsNullOrWhiteSpace(itemDto.Observaciones)) item.ActualizarObservaciones(itemDto.Observaciones);
                documento.AgregarItem(item);
            }

            // Impuestos
            foreach (var imp in dto.Impuestos)
            {
                var impuesto = new ImpuestoDocumento(
                    tipoImpuesto: imp.TipoImpuesto,
                    porcentaje:   imp.Porcentaje,
                    baseGravable: new ValorMonetario(imp.BaseGravable.Valor, imp.BaseGravable.Moneda),
                    codigoTributo:  imp.CodigoTributo,
                    nombreTributo:  imp.NombreTributo,
                    documentoId:    documento.Id);
                documento.AgregarImpuesto(impuesto);
            }

            // ── 3. Generar XML UBL 2.1 ───────────────────────────────────────────
            _logger.LogInformation("Generando XML UBL 2.1 para documento {Numero}", numeroDocumento);
            var xmlContent = await _xmlGenerator.GenerarXmlUbl21Async(documento, emisor, cliente, numeracion);
            documento.MarcarComoGenerado(xmlContent);

            // ── 4. Firmar digitalmente + calcular CUFE ───────────────────────────
            var esAmbientePruebas = _configuration.GetValue<bool>("Dian:AmbientePruebas", true);
            _logger.LogInformation("Firmando documento {Numero} (Pruebas={Pruebas})", numeroDocumento, esAmbientePruebas);
            var resultadoFirma = await _firmaDigital.FirmarDocumentoAsync(
                xmlContent, emisor, documento, cliente, esAmbientePruebas);
            documento.MarcarComoFirmado(resultadoFirma.XmlFirmado, resultadoFirma.Cufe);
            documento.GenerarQrCode(
                $"https://catalogo-vpfe{(esAmbientePruebas ? "-hab" : "")}.dian.gov.co/document/searchqr?documentkey={resultadoFirma.Cufe}");

            // ── 5. Guardar en BD ─────────────────────────────────────────────────
            await _documentoRepository.AddAsync(documento);
            await _unitOfWork.SaveChangesAsync();

            // ── 6. Transmitir a la DIAN ──────────────────────────────────────────
            _logger.LogInformation("Transmitiendo {Numero} a la DIAN", numeroDocumento);
            RespuestaDianWs respuestaDian;

            var testSetId = _configuration["Dian:TestSetId"] ?? "";
            if (esAmbientePruebas && !string.IsNullOrEmpty(testSetId) && testSetId != "CONFIGURE_YOUR_TESTSET_ID_HERE")
            {
                respuestaDian = await _dianSoap.TransmitirDocumentoPruebasAsync(
                    resultadoFirma.XmlFirmado, numeroDocumento, testSetId);
            }
            else
            {
                respuestaDian = await _dianSoap.TransmitirDocumentoAsync(
                    resultadoFirma.XmlFirmado, numeroDocumento, emisor.Codigo, esAmbientePruebas);
            }

            // Actualizar estado y respuesta DIAN en un scope nuevo para reducir riesgos de DbUpdateConcurrencyException
            // (los nuevos eventos se insertan en un contexto limpio en lugar de mezclar Added/Modified en el mismo contexto)
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IDocumentoElectronicoRepository>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var doc = await repo.GetWithAllAsync(documento.Id);
                if (doc != null)
                {
                    try
                    {
                        doc.MarcarComoTransmitido(DateTime.UtcNow);
                        doc.ProcesarRespuestaDian(
                            respuestaDian.Descripcion,
                            respuestaDian.Aceptado,
                            respuestaDian.FechaRespuesta);
                        await uow.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        // No bloqueamos la creación del documento si solo falla la actualización de eventos/estado
                        _logger.LogWarning(ex,
                            "DbUpdateConcurrencyException al actualizar estado/respuesta DIAN para documento {DocumentoId}",
                            documento.Id);
                    }
                }
            }

            // Recargar documento actualizado para respuesta y evento
            documento = await _documentoRepository.GetWithAllAsync(documento.Id)
                ?? documento;

            // ── 7. Publicar evento a Notificaciones via RabbitMQ ─────────────────
            var evento = new Facturacion.Shared.Events.FacturaTransmitidaDian(
                DocumentoId:         documento.Id,
                NumeroDocumento:     documento.NumeroDocumento,
                EmisorNit:           emisor.Codigo,
                EmisorNombre:        emisor.RazonSocial,
                EmisorEmail:         emisor.Contacto?.Email ?? "",
                ClienteIdentificacion: cliente.Codigo,
                ClienteNombre:       cliente.RazonSocial,
                ClienteEmail:        cliente.Contacto?.Email ?? "",
                Subtotal:            documento.Subtotal.Valor,
                TotalImpuestos:      documento.TotalImpuestos.Valor,
                Total:               documento.Total.Valor,
                FechaEmision:        documento.FechaEmision,
                Cufe:                documento.Cufe,
                Aceptado:            respuestaDian.Aceptado,
                RespuestaDian:       respuestaDian.Descripcion,
                TrackId:             respuestaDian.TrackId
            );

            _eventBus.Publish("factura.transmitida.dian", evento);
            _logger.LogInformation("Evento FacturaTransmitidaDian publicado para {Numero}", numeroDocumento);

            return CreatedAtAction(nameof(GetById), new { id = documento.Id }, MapToDto(documento));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando documento electrónico");
            return BadRequest(new { error = ex.Message, details = ex.InnerException?.Message });
        }
    }

    // ── POST /api/documentoselectronicos/{id}/retransmitir ────────────────────────
    /// <summary>
    /// Retransmite a la DIAN un documento ya firmado que fue rechazado o está en estado Firmado.
    /// </summary>
    [HttpPost("{id}/retransmitir")]
    public async Task<IActionResult> Retransmitir(Guid id)
    {
        try
        {
            var documento = await _documentoRepository.GetWithAllAsync(id);
            if (documento == null) return NotFound();
            if (string.IsNullOrEmpty(documento.XmlFirmado))
                return BadRequest("El documento no tiene XML firmado. Use el endpoint de creación.");

            var emisor  = await _emisorRepository.GetByNitAsync("9001234567");
            var cliente = await _clienteRepository.GetByIdentificacionAsync(documento.ClienteId);
            if (emisor == null || cliente == null) return NotFound("Emisor o cliente no encontrado.");

            var esAmbientePruebas = _configuration.GetValue<bool>("Dian:AmbientePruebas", true);
            var testSetId = _configuration["Dian:TestSetId"] ?? "";

            RespuestaDianWs respuesta;
            if (esAmbientePruebas && !string.IsNullOrEmpty(testSetId) && testSetId != "CONFIGURE_YOUR_TESTSET_ID_HERE")
                respuesta = await _dianSoap.TransmitirDocumentoPruebasAsync(documento.XmlFirmado, documento.NumeroDocumento, testSetId);
            else
                respuesta = await _dianSoap.TransmitirDocumentoAsync(documento.XmlFirmado, documento.NumeroDocumento, emisor.Codigo, esAmbientePruebas);

            documento.MarcarComoTransmitido(DateTime.UtcNow);
            documento.ProcesarRespuestaDian(respuesta.Descripcion, respuesta.Aceptado, respuesta.FechaRespuesta);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new
            {
                documentoId = id,
                numeroDocumento = documento.NumeroDocumento,
                aceptado = respuesta.Aceptado,
                codigoEstado = respuesta.CodigoEstado,
                descripcion = respuesta.Descripcion,
                trackId = respuesta.TrackId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ── GET /api/documentoselectronicos/{id}/xml ──────────────────────────────────
    [HttpGet("{id}/xml")]
    public async Task<IActionResult> DescargarXml(Guid id)
    {
        var documento = await _documentoRepository.GetWithAllAsync(id);
        if (documento == null) return NotFound();

        var xml = documento.XmlFirmado ?? documento.XmlContent;
        if (string.IsNullOrEmpty(xml)) return NotFound("El documento no tiene XML generado.");

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
        return File(bytes, "application/xml", $"{documento.NumeroDocumento}.xml");
    }

    // ── GET /api/documentoselectronicos/{id} ──────────────────────────────────────
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var documento = await _documentoRepository.GetWithAllAsync(id);
        if (documento == null) return NotFound();
        return Ok(MapToDto(documento));
    }

    // ── GET /api/documentoselectronicos ───────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var documentos = await _documentoRepository.GetAllAsync();
        var list = documentos.Select(MapToDto).ToList();
        return Ok(new { total = list.Count, page, pageSize, data = list.Skip((page - 1) * pageSize).Take(pageSize) });
    }

    [HttpGet("emisor/{emisorId}")]
    public async Task<IActionResult> GetByEmisor(string emisorId, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
    {
        var documentos = await _documentoRepository.GetByEmisorAsync(emisorId, fechaInicio, fechaFin);
        return Ok(documentos.Select(MapToDto).ToList());
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> GetByCliente(string clienteId, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
    {
        var documentos = await _documentoRepository.GetByClienteAsync(clienteId, fechaInicio, fechaFin);
        return Ok(documentos.Select(MapToDto).ToList());
    }

    // ── Mapper ───────────────────────────────────────────────────────────────────
    private static DocumentoElectronicoDTO MapToDto(DocumentoElectronico d) => new()
    {
        Id = d.Id, NumeroDocumento = d.NumeroDocumento, TipoDocumento = d.TipoDocumento,
        Estado = d.Estado, FechaEmision = d.FechaEmision, FechaVencimiento = d.FechaVencimiento,
        FechaEntrega = d.FechaEntrega, Observaciones = d.Observaciones, Cufe = d.Cufe,
        QrCode = d.QrCode, RespuestaDian = d.RespuestaDian, FechaTransmision = d.FechaTransmision,
        FechaRespuesta = d.FechaRespuesta, EmisorId = d.EmisorId, ClienteId = d.ClienteId,
        Subtotal          = new ValorMonetarioDTO { Valor = d.Subtotal.Valor,           Moneda = d.Subtotal.Moneda },
        TotalDescuentos   = new ValorMonetarioDTO { Valor = d.TotalDescuentos.Valor,    Moneda = d.TotalDescuentos.Moneda },
        TotalImpuestos    = new ValorMonetarioDTO { Valor = d.TotalImpuestos.Valor,     Moneda = d.TotalImpuestos.Moneda },
        Total             = new ValorMonetarioDTO { Valor = d.Total.Valor,              Moneda = d.Total.Moneda },
        TotalPagado       = new ValorMonetarioDTO { Valor = d.TotalPagado.Valor,        Moneda = d.TotalPagado.Moneda },
        SaldoPendiente    = new ValorMonetarioDTO { Valor = d.SaldoPendiente.Valor,     Moneda = d.SaldoPendiente.Moneda },
        Items = d.Items.Select(i => new ItemDocumentoDTO
        {
            Id = i.Id, Orden = i.Orden, Codigo = i.Codigo, Descripcion = i.Descripcion,
            CodigoEstandar = i.CodigoEstandar, Cantidad = i.Cantidad, UnidadMedida = i.UnidadMedida,
            ValorUnitario = new ValorMonetarioDTO { Valor = i.ValorUnitario.Valor, Moneda = i.ValorUnitario.Moneda },
            ValorTotal    = new ValorMonetarioDTO { Valor = i.ValorTotal.Valor,    Moneda = i.ValorTotal.Moneda },
            Descuento     = i.Descuento != null ? new ValorMonetarioDTO { Valor = i.Descuento.Valor, Moneda = i.Descuento.Moneda } : null,
            Observaciones = i.Observaciones
        }).ToList(),
        Impuestos = d.Impuestos.Select(i => new ImpuestoDocumentoDTO
        {
            Id = i.Id, TipoImpuesto = i.TipoImpuesto, Porcentaje = i.Porcentaje,
            BaseGravable  = new ValorMonetarioDTO { Valor = i.BaseGravable.Valor, Moneda = i.BaseGravable.Moneda },
            Valor         = new ValorMonetarioDTO { Valor = i.Valor.Valor,        Moneda = i.Valor.Moneda },
            CodigoTributo = i.CodigoTributo, NombreTributo = i.NombreTributo
        }).ToList(),
        Pagos = d.Pagos.Select(p => new PagoDocumentoDTO
        {
            Id = p.Id, MetodoPago = p.MetodoPago,
            Valor = new ValorMonetarioDTO { Valor = p.Valor.Valor, Moneda = p.Valor.Moneda },
            FechaPago = p.FechaPago, Referencia = p.Referencia, Observaciones = p.Observaciones
        }).ToList(),
        Eventos = d.Eventos.Select(e => new EventoDocumentoDTO
        {
            Id = e.Id, TipoEvento = e.TipoEvento, Descripcion = e.Descripcion,
            Detalles = e.Detalles, Usuario = e.Usuario, FechaCreacion = e.FechaCreacion
        }).ToList()
    };
}
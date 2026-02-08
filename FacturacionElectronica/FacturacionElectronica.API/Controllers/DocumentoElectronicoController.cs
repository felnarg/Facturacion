using FacturacionElectronica.Application.DTOs;
using FacturacionElectronica.Domain.Entities;
using FacturacionElectronica.Domain.Enums;
using FacturacionElectronica.Domain.Interfaces;
using FacturacionElectronica.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacturacionElectronica.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentoElectronicoController : ControllerBase
    {
        private readonly IDocumentoElectronicoRepository _documentoRepository;
        private readonly IEmisorRepository _emisorRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly INumeracionDocumentoRepository _numeracionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DocumentoElectronicoController(
            IDocumentoElectronicoRepository documentoRepository,
            IEmisorRepository emisorRepository,
            IClienteRepository clienteRepository,
            INumeracionDocumentoRepository numeracionRepository,
            IUnitOfWork unitOfWork)
        {
            _documentoRepository = documentoRepository;
            _emisorRepository = emisorRepository;
            _clienteRepository = clienteRepository;
            _numeracionRepository = numeracionRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> CrearDocumento([FromBody] CrearDocumentoElectronicoDTO dto)
        {
            try
            {
                // Obtener emisor por defecto (del seed)
                var emisor = await _emisorRepository.GetByNitAsync("9001234567");
                if (emisor == null)
                    return NotFound("No se encontró el emisor. Asegúrate de que el seed se haya ejecutado.");

                // Obtener cliente
                var cliente = await _clienteRepository.GetByIdentificacionAsync(dto.ClienteId);
                if (cliente == null)
                    return NotFound($"No se encontró el cliente con identificación: {dto.ClienteId}");

                // Obtener numeración activa para el tipo de documento
                var numeracion = await _numeracionRepository.GetActivaByEmisorAndTipoAsync(
                    emisor.Codigo, 
                    dto.TipoDocumento);

                if (numeracion == null)
                    return NotFound($"No se encontró numeración activa para el tipo de documento: {dto.TipoDocumento}");

                // Generar número de documento
                var numeroDocumento = numeracion.ObtenerSiguienteNumero();

                // Crear documento
                var documento = new DocumentoElectronico(
                    numeroDocumento: numeroDocumento,
                    tipoDocumento: dto.TipoDocumento,
                    fechaEmision: dto.FechaEmision,
                    fechaVencimiento: dto.FechaVencimiento,
                    emisorId: emisor.Codigo,
                    clienteId: cliente.Codigo
                );

                if (!string.IsNullOrWhiteSpace(dto.Observaciones))
                    documento.ActualizarObservaciones(dto.Observaciones);

                // Agregar items
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
                        documentoId: documento.Id
                    );

                    if (!string.IsNullOrWhiteSpace(itemDto.CodigoEstandar))
                        item.ActualizarCodigoEstandar(itemDto.CodigoEstandar);

                    if (itemDto.Descuento != null)
                        item.AplicarDescuento(new ValorMonetario(itemDto.Descuento.Valor, itemDto.Descuento.Moneda));

                    if (!string.IsNullOrWhiteSpace(itemDto.Observaciones))
                        item.ActualizarObservaciones(itemDto.Observaciones);

                    documento.AgregarItem(item);
                }

                // Agregar impuestos
                foreach (var impuestoDto in dto.Impuestos)
                {
                    var impuesto = new ImpuestoDocumento(
                        tipoImpuesto: impuestoDto.TipoImpuesto,
                        porcentaje: impuestoDto.Porcentaje,
                        baseGravable: new ValorMonetario(impuestoDto.BaseGravable.Valor, impuestoDto.BaseGravable.Moneda),
                        codigoTributo: impuestoDto.CodigoTributo,
                        nombreTributo: impuestoDto.NombreTributo,
                        documentoId: documento.Id
                    );

                    documento.AgregarImpuesto(impuesto);
                }

                // Guardar documento
                await _documentoRepository.AddAsync(documento);
                await _unitOfWork.SaveChangesAsync();

                // Mapear a DTO para respuesta
                var documentoDto = new DocumentoElectronicoDTO
                {
                    Id = documento.Id,
                    NumeroDocumento = documento.NumeroDocumento,
                    TipoDocumento = documento.TipoDocumento,
                    Estado = documento.Estado,
                    FechaEmision = documento.FechaEmision,
                    FechaVencimiento = documento.FechaVencimiento,
                    Observaciones = documento.Observaciones,
                    EmisorId = documento.EmisorId,
                    ClienteId = documento.ClienteId,
                    Subtotal = new ValorMonetarioDTO { Valor = documento.Subtotal.Valor, Moneda = documento.Subtotal.Moneda },
                    TotalDescuentos = new ValorMonetarioDTO { Valor = documento.TotalDescuentos.Valor, Moneda = documento.TotalDescuentos.Moneda },
                    TotalImpuestos = new ValorMonetarioDTO { Valor = documento.TotalImpuestos.Valor, Moneda = documento.TotalImpuestos.Moneda },
                    Total = new ValorMonetarioDTO { Valor = documento.Total.Valor, Moneda = documento.Total.Moneda },
                    TotalPagado = new ValorMonetarioDTO { Valor = documento.TotalPagado.Valor, Moneda = documento.TotalPagado.Moneda },
                    SaldoPendiente = new ValorMonetarioDTO { Valor = documento.SaldoPendiente.Valor, Moneda = documento.SaldoPendiente.Moneda }
                };

                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = documento.Id }, 
                    documentoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var documento = await _documentoRepository.GetWithAllAsync(id);
            if (documento == null)
                return NotFound();

            var dto = MapToDto(documento);
            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var documentos = await _documentoRepository.GetAllAsync();
            var documentosDto = documentos.Select(MapToDto).ToList();
            
            return Ok(new
            {
                total = documentosDto.Count,
                page,
                pageSize,
                data = documentosDto.Skip((page - 1) * pageSize).Take(pageSize)
            });
        }

        [HttpGet("emisor/{emisorId}")]
        public async Task<IActionResult> GetByEmisor(string emisorId, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            var documentos = await _documentoRepository.GetByEmisorAsync(emisorId, fechaInicio, fechaFin);
            var documentosDto = documentos.Select(MapToDto).ToList();
            return Ok(documentosDto);
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetByCliente(string clienteId, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            var documentos = await _documentoRepository.GetByClienteAsync(clienteId, fechaInicio, fechaFin);
            var documentosDto = documentos.Select(MapToDto).ToList();
            return Ok(documentosDto);
        }

        private DocumentoElectronicoDTO MapToDto(DocumentoElectronico documento)
        {
            return new DocumentoElectronicoDTO
            {
                Id = documento.Id,
                NumeroDocumento = documento.NumeroDocumento,
                TipoDocumento = documento.TipoDocumento,
                Estado = documento.Estado,
                FechaEmision = documento.FechaEmision,
                FechaVencimiento = documento.FechaVencimiento,
                FechaEntrega = documento.FechaEntrega,
                Observaciones = documento.Observaciones,
                Cufe = documento.Cufe,
                QrCode = documento.QrCode,
                RespuestaDian = documento.RespuestaDian,
                FechaTransmision = documento.FechaTransmision,
                FechaRespuesta = documento.FechaRespuesta,
                EmisorId = documento.EmisorId,
                ClienteId = documento.ClienteId,
                Subtotal = new ValorMonetarioDTO { Valor = documento.Subtotal.Valor, Moneda = documento.Subtotal.Moneda },
                TotalDescuentos = new ValorMonetarioDTO { Valor = documento.TotalDescuentos.Valor, Moneda = documento.TotalDescuentos.Moneda },
                TotalImpuestos = new ValorMonetarioDTO { Valor = documento.TotalImpuestos.Valor, Moneda = documento.TotalImpuestos.Moneda },
                Total = new ValorMonetarioDTO { Valor = documento.Total.Valor, Moneda = documento.Total.Moneda },
                TotalPagado = new ValorMonetarioDTO { Valor = documento.TotalPagado.Valor, Moneda = documento.TotalPagado.Moneda },
                SaldoPendiente = new ValorMonetarioDTO { Valor = documento.SaldoPendiente.Valor, Moneda = documento.SaldoPendiente.Moneda },
                Items = documento.Items.Select(i => new ItemDocumentoDTO
                {
                    Id = i.Id,
                    Orden = i.Orden,
                    Codigo = i.Codigo,
                    Descripcion = i.Descripcion,
                    CodigoEstandar = i.CodigoEstandar,
                    Cantidad = i.Cantidad,
                    UnidadMedida = i.UnidadMedida,
                    ValorUnitario = new ValorMonetarioDTO { Valor = i.ValorUnitario.Valor, Moneda = i.ValorUnitario.Moneda },
                    ValorTotal = new ValorMonetarioDTO { Valor = i.ValorTotal.Valor, Moneda = i.ValorTotal.Moneda },
                    Descuento = i.Descuento != null ? new ValorMonetarioDTO { Valor = i.Descuento.Valor, Moneda = i.Descuento.Moneda } : null,
                    Observaciones = i.Observaciones
                }).ToList(),
                Impuestos = documento.Impuestos.Select(i => new ImpuestoDocumentoDTO
                {
                    Id = i.Id,
                    TipoImpuesto = i.TipoImpuesto,
                    Porcentaje = i.Porcentaje,
                    BaseGravable = new ValorMonetarioDTO { Valor = i.BaseGravable.Valor, Moneda = i.BaseGravable.Moneda },
                    Valor = new ValorMonetarioDTO { Valor = i.Valor.Valor, Moneda = i.Valor.Moneda },
                    CodigoTributo = i.CodigoTributo,
                    NombreTributo = i.NombreTributo
                }).ToList(),
                Pagos = documento.Pagos.Select(p => new PagoDocumentoDTO
                {
                    Id = p.Id,
                    MetodoPago = p.MetodoPago,
                    Valor = new ValorMonetarioDTO { Valor = p.Valor.Valor, Moneda = p.Valor.Moneda },
                    FechaPago = p.FechaPago,
                    Referencia = p.Referencia,
                    Observaciones = p.Observaciones
                }).ToList(),
                Eventos = documento.Eventos.Select(e => new EventoDocumentoDTO
                {
                    Id = e.Id,
                    TipoEvento = e.TipoEvento,
                    Descripcion = e.Descripcion,
                    Detalles = e.Detalles,
                    Usuario = e.Usuario,
                    FechaCreacion = e.FechaCreacion
                }).ToList()
            };
        }
    }
}
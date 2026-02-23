using Facturacion.Shared.Events;
using Microsoft.Extensions.Logging;
using Notificaciones.Application.Templates;
using Notificaciones.Domain.Entities;
using Notificaciones.Domain.Enums;
using Notificaciones.Domain.Interfaces;

namespace Notificaciones.Application.EventHandlers;

/// <summary>
/// Maneja el evento FacturaTransmitidaDian publicado por FacturacionElectronica.
/// Responsabilidades: construir email, enviarlo y registrar la auditoría.
/// Sigue SRP y el principio de Open/Closed — agregar canales no modifica este handler.
/// </summary>
public sealed class FacturaTransmitidaDianHandler
{
    private readonly IEmailSender _emailSender;
    private readonly INotificacionRepository _repository;
    private readonly ILogger<FacturaTransmitidaDianHandler> _logger;

    public FacturaTransmitidaDianHandler(
        IEmailSender emailSender,
        INotificacionRepository repository,
        ILogger<FacturaTransmitidaDianHandler> logger)
    {
        _emailSender = emailSender;
        _repository  = repository;
        _logger      = logger;
    }

    /// <summary>
    /// Procesa el evento: envía email al cliente y registra la auditoría.
    /// </summary>
    public async Task HandleAsync(FacturaTransmitidaDian evento)
    {
        _logger.LogInformation(
            "[Notificaciones] Procesando FacturaTransmitidaDian: {Numero} | Aceptado={Aceptado} | Cliente={Cliente}",
            evento.NumeroDocumento, evento.Aceptado, evento.ClienteEmail);

        // Sólo notificar si el cliente tiene email registrado
        if (string.IsNullOrWhiteSpace(evento.ClienteEmail))
        {
            _logger.LogWarning("[Notificaciones] Cliente sin email. Documento: {Numero}", evento.NumeroDocumento);
            return;
        }

        var asunto   = evento.Aceptado
            ? $"✅ Factura {evento.NumeroDocumento} aceptada por la DIAN"
            : $"❌ Factura {evento.NumeroDocumento} rechazada por la DIAN";

        var notificacion = new Notificacion(
            documentoId:       evento.DocumentoId,
            numeroDocumento:   evento.NumeroDocumento,
            destinatario:      evento.ClienteEmail,
            nombreDestinatario:evento.ClienteNombre,
            tipo:              evento.Aceptado ? TipoNotificacion.FacturaAceptadaDian : TipoNotificacion.FacturaRechazadaDian,
            canal:             CanalNotificacion.Email,
            asunto:            asunto);

        await _repository.AddAsync(notificacion);
        await _repository.SaveChangesAsync();

        try
        {
            var html = FacturaClienteHtmlTemplate.Generar(
                nombreCliente:   evento.ClienteNombre,
                numeroDocumento: evento.NumeroDocumento,
                cufe:            evento.Cufe,
                fechaEmision:    evento.FechaEmision,
                subtotal:        evento.Subtotal,
                totalImpuestos:  evento.TotalImpuestos,
                total:           evento.Total,
                aceptado:        evento.Aceptado,
                respuestaDian:   evento.RespuestaDian,
                emisorNombre:    evento.EmisorNombre);

            var enviado = await _emailSender.EnviarAsync(
                evento.ClienteEmail,
                evento.ClienteNombre,
                asunto,
                html);

            if (enviado)
            {
                notificacion.MarcarComoEnviada();
                _logger.LogInformation(
                    "[Notificaciones] Email enviado a {Email} para factura {Numero}",
                    evento.ClienteEmail, evento.NumeroDocumento);
            }
            else
            {
                notificacion.MarcarComoFallida("El proveedor SMTP rechazó el mensaje.");
                _logger.LogError(
                    "[Notificaciones] Fallo al enviar email a {Email} para factura {Numero}",
                    evento.ClienteEmail, evento.NumeroDocumento);
            }
        }
        catch (Exception ex)
        {
            notificacion.MarcarComoFallida(ex.Message);
            _logger.LogError(ex,
                "[Notificaciones] Excepción enviando email para {Numero}", evento.NumeroDocumento);
        }
        finally
        {
            await _repository.SaveChangesAsync();
        }
    }
}

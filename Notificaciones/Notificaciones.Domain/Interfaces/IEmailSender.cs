namespace Notificaciones.Domain.Interfaces;

/// <summary>
/// Contrato del servicio de envío de correo electrónico.
/// Ubicado en el Dominio para que Application dependa de la abstracción,
/// no de MailKit ni de ninguna implementación concreta.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Envía un correo HTML.
    /// </summary>
    /// <returns>true si el envío fue exitoso.</returns>
    Task<bool> EnviarAsync(
        string destinatario,
        string nombreDestinatario,
        string asunto,
        string cuerpoHtml);
}

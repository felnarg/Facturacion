using Notificaciones.Domain.Enums;

namespace Notificaciones.Domain.Entities;

/// <summary>
/// Entidad de auditoría que registra cada intento de notificación.
/// Cumple con SRP: solo responsabilidad de llevar el historial de envíos.
/// </summary>
public class Notificacion
{
    public Guid   Id                  { get; private set; }
    public Guid   DocumentoId         { get; private set; }
    public string NumeroDocumento     { get; private set; } = string.Empty;
    public string Destinatario        { get; private set; } = string.Empty;
    public string NombreDestinatario  { get; private set; } = string.Empty;
    public TipoNotificacion    Tipo   { get; private set; }
    public CanalNotificacion   Canal  { get; private set; }
    public EstadoNotificacion  Estado { get; private set; }
    public string Asunto              { get; private set; } = string.Empty;
    public DateTime FechaIntento      { get; private set; }
    public DateTime? FechaEnvio       { get; private set; }
    public string? ErrorMensaje       { get; private set; }
    public int Intentos               { get; private set; }

    private Notificacion() { }

    public Notificacion(
        Guid documentoId,
        string numeroDocumento,
        string destinatario,
        string nombreDestinatario,
        TipoNotificacion tipo,
        CanalNotificacion canal,
        string asunto)
    {
        Id                 = Guid.NewGuid();
        DocumentoId        = documentoId;
        NumeroDocumento    = numeroDocumento;
        Destinatario       = destinatario;
        NombreDestinatario = nombreDestinatario;
        Tipo               = tipo;
        Canal              = canal;
        Estado             = EstadoNotificacion.Pendiente;
        Asunto             = asunto;
        FechaIntento       = DateTime.UtcNow;
        Intentos           = 0;
    }

    public void MarcarComoEnviada()
    {
        Estado    = EstadoNotificacion.Enviada;
        FechaEnvio= DateTime.UtcNow;
        Intentos++;
    }

    public void MarcarComoFallida(string error)
    {
        Estado       = EstadoNotificacion.Fallida;
        ErrorMensaje = error;
        Intentos++;
    }

    public void IncrementarIntento()
    {
        Estado   = EstadoNotificacion.Reintentando;
        Intentos++;
    }
}

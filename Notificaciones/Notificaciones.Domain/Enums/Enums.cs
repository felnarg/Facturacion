namespace Notificaciones.Domain.Enums;

public enum TipoNotificacion
{
    FacturaCliente       = 1,   // Email al cliente con resumen de su factura
    FacturaAceptadaDian  = 2,   // Email al cliente confirmando aceptación DIAN
    FacturaRechazadaDian = 3    // Email al admin avisando rechazo DIAN
}

public enum EstadoNotificacion
{
    Pendiente    = 1,
    Enviada      = 2,
    Fallida      = 3,
    Reintentando = 4
}

public enum CanalNotificacion
{
    Email = 1,
    SMS   = 2   // Futuro
}

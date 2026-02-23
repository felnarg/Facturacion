using Microsoft.AspNetCore.Mvc;
using Notificaciones.Domain.Interfaces;

namespace Notificaciones.API.Controllers;

[ApiController]
[Route("api/notificaciones")]
public class HealthController : ControllerBase
{
    private readonly INotificacionRepository _repository;

    public HealthController(INotificacionRepository repository)
        => _repository = repository;

    /// <summary>GET /api/notificaciones/health</summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status    = "ok",
            servicio  = "Notificaciones",
            timestamp = DateTime.UtcNow,
            version   = "1.0.0"
        });
    }

    /// <summary>GET /api/notificaciones/documento/{documentoId}</summary>
    [HttpGet("documento/{documentoId:guid}")]
    public async Task<IActionResult> GetByDocumento(Guid documentoId)
    {
        var notificaciones = await _repository.GetByDocumentoIdAsync(documentoId);
        return Ok(notificaciones);
    }
}

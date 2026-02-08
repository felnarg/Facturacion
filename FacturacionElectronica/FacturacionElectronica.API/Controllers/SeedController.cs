using FacturacionElectronica.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacturacionElectronica.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SeedController> _logger;

        public SeedController(ApplicationDbContext context, ILogger<SeedController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var emisoresCount = await _context.Emisores.CountAsync();
                var clientesCount = await _context.Clientes.CountAsync();
                var numeracionesCount = await _context.NumeracionesDocumentos.CountAsync();
                var documentosCount = await _context.DocumentosElectronicos.CountAsync();

                return Ok(new
                {
                    databaseConnected = canConnect,
                    emisores = emisoresCount,
                    clientes = clientesCount,
                    numeraciones = numeracionesCount,
                    documentos = documentosCount,
                    tieneDatos = emisoresCount > 0 || clientesCount > 0 || numeracionesCount > 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpPost("force")]
        public async Task<IActionResult> ForceSeed()
        {
            try
            {
                _logger.LogInformation("Forzando seed de datos...");

                // Limpiar datos existentes (solo para desarrollo)
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    _logger.LogWarning("Modo desarrollo: Limpiando datos existentes...");
                    _context.Emisores.RemoveRange(await _context.Emisores.ToListAsync());
                    _context.Clientes.RemoveRange(await _context.Clientes.ToListAsync());
                    _context.NumeracionesDocumentos.RemoveRange(await _context.NumeracionesDocumentos.ToListAsync());
                    await _context.SaveChangesAsync();
                }

                // Ejecutar seed
                await SeedData.Initialize(_context, _logger);

                var emisoresCount = await _context.Emisores.CountAsync();
                var clientesCount = await _context.Clientes.CountAsync();
                var numeracionesCount = await _context.NumeracionesDocumentos.CountAsync();

                return Ok(new
                {
                    message = "Seed ejecutado exitosamente",
                    emisores = emisoresCount,
                    clientes = clientesCount,
                    numeraciones = numeracionesCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar seed forzado");
                return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
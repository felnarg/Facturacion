using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notificaciones.Application.EventHandlers;
using Notificaciones.Infrastructure.EventBus;

namespace Notificaciones.API.Workers;

/// <summary>
/// BackgroundService que mantiene la conexión a RabbitMQ activa
/// y despacha los mensajes al FacturaTransmitidaDianHandler.
/// Se reconecta automáticamente si RabbitMQ no está disponible al iniciar.
/// </summary>
public sealed class EventConsumerWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EventConsumerWorker> _logger;

    public EventConsumerWorker(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<EventConsumerWorker> logger)
    {
        _scopeFactory  = scopeFactory;
        _configuration = configuration;
        _logger        = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Worker] EventConsumerWorker iniciando...");

        // Esperar a RabbitMQ con reintentos (máximo 5 intentos)
        RabbitMQConsumer? consumer = null;
        for (int intento = 1; intento <= 5; intento++)
        {
            try
            {
                consumer = CrearConsumer();
                _logger.LogInformation("[Worker] Conexión a RabbitMQ establecida.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[Worker] Intento {N}/5 fallido: {Msg}. Reintentando en 5s...", intento, ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        if (consumer is null)
        {
            _logger.LogError("[Worker] No se pudo conectar a RabbitMQ tras 5 intentos. Worker detenido.");
            return;
        }

        // Iniciar consumo: por cada mensaje, crear un scope DI y ejecutar el handler
        consumer.StartConsuming(async evento =>
        {
            using var scope = _scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<FacturaTransmitidaDianHandler>();
            await handler.HandleAsync(evento);
        });

        // Mantener el worker vivo
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        consumer.Dispose();
        _logger.LogInformation("[Worker] EventConsumerWorker detenido.");
    }

    private RabbitMQConsumer CrearConsumer()
    {
        var logger = _scopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<ILogger<RabbitMQConsumer>>();

        return new RabbitMQConsumer(
            hostName:     _configuration["RabbitMQ:HostName"]  ?? "rabbitmq",
            userName:     _configuration["RabbitMQ:UserName"]  ?? "admin",
            password:     _configuration["RabbitMQ:Password"]  ?? "p@ssword123",
            exchangeName: _configuration["RabbitMQ:Exchange"]  ?? "facturacion.events",
            queueName:    _configuration["RabbitMQ:Queue"]     ?? "notificaciones.events",
            logger:       logger);
    }
}

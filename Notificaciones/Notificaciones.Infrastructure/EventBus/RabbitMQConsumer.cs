using Facturacion.Shared.Events;
using Microsoft.Extensions.Logging;
using Notificaciones.Application.EventHandlers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Notificaciones.Infrastructure.EventBus;

/// <summary>
/// Suscribe a la cola de RabbitMQ "notificaciones.events" y despacha los
/// mensajes al handler correspondiente.
/// </summary>
public sealed class RabbitMQConsumer : IDisposable
{
    private const string RoutingKey = "factura.transmitida.dian";

    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly string _exchangeName;
    private readonly ILogger<RabbitMQConsumer> _logger;

    public RabbitMQConsumer(
        string hostName,
        string userName,
        string password,
        string exchangeName,
        string queueName,
        ILogger<RabbitMQConsumer> logger)
    {
        _exchangeName = exchangeName;
        _queueName    = queueName;
        _logger       = logger;

        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection("notificaciones-consumer");
        _channel    = _connection.CreateModel();

        // Declarar el exchange (idempotente)
        _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);

        // Declarar la cola de notificaciones
        _channel.QueueDeclare(
            queue:      _queueName,
            durable:    true,
            exclusive:  false,
            autoDelete: false);

        // Bind a la routing key del evento
        _channel.QueueBind(
            queue:      _queueName,
            exchange:   _exchangeName,
            routingKey: RoutingKey);

        // Prefetch = 1: procesar un mensaje a la vez (control de carga)
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        _logger.LogInformation(
            "[RabbitMQ] Consumer inicializado. Exchange={Exchange}, Queue={Queue}, RoutingKey={Key}",
            _exchangeName, _queueName, RoutingKey);
    }

    /// <summary>
    /// Inicia el consumo asíncrono de mensajes.
    /// Llama al handler por cada mensaje recibido.
    /// </summary>
    public void StartConsuming(Func<FacturaTransmitidaDian, Task> handler)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body    = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("[RabbitMQ] Mensaje recibido. RoutingKey={Key}", ea.RoutingKey);

            try
            {
                var evento = JsonSerializer.Deserialize<FacturaTransmitidaDian>(message,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (evento is null)
                {
                    _logger.LogError("[RabbitMQ] No se pudo deserializar el mensaje: {Msg}", message[..Math.Min(200, message.Length)]);
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: false);
                    return;
                }

                await handler(evento);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[RabbitMQ] Error procesando mensaje. Reencolar.");
                _channel.BasicNack(ea.DeliveryTag, false, requeue: true);
            }
        };

        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("[RabbitMQ] Consumidor activo en queue={Queue}", _queueName);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}

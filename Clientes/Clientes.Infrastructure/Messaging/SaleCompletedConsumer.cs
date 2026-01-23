using System.Text.Json;
using Clientes.Application.Abstractions;
using Facturacion.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Clientes.Infrastructure.Messaging;

public sealed class SaleCompletedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<SaleCompletedConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public SaleCompletedConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<SaleCompletedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(_options.Queue, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(_options.Queue, _options.Exchange, "sale.completed");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var saleHistoryService = scope.ServiceProvider.GetRequiredService<ISaleHistoryService>();

                var message = JsonSerializer.Deserialize<SaleCompleted>(args.Body.ToArray());
                if (message is not null)
                {
                    var itemsCount = message.Items.Sum(item => item.Quantity);
                    await saleHistoryService.AddAsync(message.SaleId, itemsCount, message.OccurredOn, stoppingToken);
                }

                _channel.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando sale.completed");
                _channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(_options.Queue, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}

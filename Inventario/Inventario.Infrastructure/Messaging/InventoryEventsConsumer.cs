using System.Text;
using System.Text.Json;
using Facturacion.Shared.Events;
using Inventario.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Inventario.Infrastructure.Messaging;

public sealed class InventoryEventsConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<InventoryEventsConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public InventoryEventsConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<InventoryEventsConsumer> logger)
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
        _channel.QueueBind(_options.Queue, _options.Exchange, "product.created");
        _channel.QueueBind(_options.Queue, _options.Exchange, "product.updated");
        _channel.QueueBind(_options.Queue, _options.Exchange, "stock.received");
        _channel.QueueBind(_options.Queue, _options.Exchange, "sale.completed");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                await HandleMessageAsync(args.RoutingKey, args.Body.ToArray(), stoppingToken);
                _channel.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando evento {RoutingKey}", args.RoutingKey);
                _channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(_options.Queue, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(string routingKey, byte[] body, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var stockService = scope.ServiceProvider.GetRequiredService<IStockService>();

        switch (routingKey)
        {
            case "product.created":
            {
                var message = JsonSerializer.Deserialize<ProductCreated>(body);
                if (message is not null)
                {
                    await stockService.CreateForProductAsync(message.ProductId, cancellationToken);
                }

                break;
            }
            case "product.updated":
            {
                // Product updates no longer carry stock information (Stock is now mastered in Inventory).
                // We do nothing here regarding stock.
                break;
            }
            case "stock.received":
            {
                var message = JsonSerializer.Deserialize<StockReceived>(body);
                if (message is not null)
                {
                    await stockService.IncreaseAsync(message.ProductId, message.Quantity, message.PurchaseId, cancellationToken);
                }

                break;
            }
            case "sale.completed":
            {
                var message = JsonSerializer.Deserialize<SaleCompleted>(body);
                if (message is not null)
                {
                    foreach (var item in message.Items)
                    {
                        await stockService.DecreaseAsync(item.ProductId, item.Quantity, message.SaleId, cancellationToken);
                    }
                }

                break;
            }
            default:
                _logger.LogWarning("RoutingKey no soportado: {RoutingKey}", routingKey);
                break;
        }
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

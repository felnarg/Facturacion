using System.Text.Json;
using Catalogo.Domain.Repositories;
using Facturacion.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Catalogo.Infrastructure.Messaging;

public sealed class SalePercentageEventsConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<SalePercentageEventsConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public SalePercentageEventsConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<SalePercentageEventsConsumer> logger)
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
        _channel.QueueBind(_options.Queue, _options.Exchange, "product.salepercentage.updated");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                await HandleMessageAsync(args.Body.ToArray(), stoppingToken);
                _channel.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando product.salepercentage.updated");
                _channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(_options.Queue, autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(byte[] body, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

        var message = JsonSerializer.Deserialize<PurchaseSalePercentagesUpdated>(body);
        if (message is null || message.Items.Count == 0)
        {
            return;
        }

        foreach (var item in message.Items)
        {
            var product = await repository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
            {
                continue;
            }

            if (product.SalePercentage != item.SalePercentage)
            {
                product.UpdateSalePercentage(item.SalePercentage);
                await repository.UpdateAsync(product, cancellationToken);
            }
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

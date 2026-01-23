using System.Text.Json;
using Compras.Application.Abstractions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Compras.Infrastructure.Messaging;

public sealed class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly Lazy<IConnection> _connection;
    private bool _disposed;

    public RabbitMqEventBus(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
        _connection = new Lazy<IConnection>(CreateConnection);
    }

    public Task PublishAsync<T>(T @event, string routingKey, CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(RabbitMqEventBus));
        }

        using var channel = _connection.Value.CreateModel();
        channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true);

        var body = JsonSerializer.SerializeToUtf8Bytes(@event);
        var properties = channel.CreateBasicProperties();
        properties.ContentType = "application/json";
        properties.DeliveryMode = 2;

        channel.BasicPublish(
            exchange: _options.Exchange,
            routingKey: routingKey,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_connection.IsValueCreated)
        {
            _connection.Value.Dispose();
        }

        _disposed = true;
    }

    private IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        return factory.CreateConnection();
    }
}

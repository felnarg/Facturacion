using System.Text.Json;
using Facturacion.Shared.Events;
using Kardex.Domain.Entities;
using Kardex.Domain.Enums;
using Kardex.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Kardex.Infrastructure.Messaging;

public sealed class CustomerCreditApprovedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<CustomerCreditApprovedConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public CustomerCreditApprovedConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<CustomerCreditApprovedConsumer> logger)
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

        var queueName = $"{_options.Queue}.credit-accounts";
        _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queueName, _options.Exchange, "customer.credit.approved");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ICreditAccountRepository>();

                var message = JsonSerializer.Deserialize<CustomerCreditApproved>(args.Body.ToArray());
                if (message is not null)
                {
                    var identificationType = Enum.Parse<IdentificationType>(message.IdentificationType, true);
                    var account = await repository.GetByIdentificationAsync(
                        identificationType,
                        message.IdentificationNumber,
                        stoppingToken);

                    if (account is null)
                    {
                        account = new CreditAccount(
                            message.CustomerName,
                            identificationType,
                            message.IdentificationNumber,
                            message.CreditLimit,
                            message.PaymentTermDays);
                        await repository.AddAsync(account, stoppingToken);
                    }
                    else
                    {
                        account.UpdateDetails(message.CustomerName, message.CreditLimit, message.PaymentTermDays);
                        await repository.UpdateAsync(account, stoppingToken);
                    }
                }

                _channel.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando customer.credit.approved");
                _channel?.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(queueName, autoAck: false, consumer: consumer);
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

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FacturacionElectronica.Infrastructure.EventBus
{
    public interface IEventBus
    {
        void Publish(string eventName, object eventData);
        void Subscribe(string eventName, Action<string> handler);
        void Dispose();
    }

    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly Dictionary<string, List<Action<string>>> _handlers = new();

        public RabbitMQEventBus(string hostName, string userName, string password, string exchangeName, string queueName)
        {
            _exchangeName = exchangeName;
            _queueName = queueName;

            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declarar exchange
            _channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            // Declarar queue
            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Bind queue to exchange
            _channel.QueueBind(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: "#"); // Recibir todos los eventos

            // Configurar consumidor
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventName = ea.RoutingKey;

                ProcessEvent(eventName, message);

                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);
        }

        public void Publish(string eventName, object eventData)
        {
            var message = JsonSerializer.Serialize(eventData);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: eventName,
                basicProperties: null,
                body: body);
        }

        public void Subscribe(string eventName, Action<string> handler)
        {
            if (!_handlers.ContainsKey(eventName))
            {
                _handlers[eventName] = new List<Action<string>>();
            }

            _handlers[eventName].Add(handler);
        }

        private void ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                foreach (var handler in _handlers[eventName])
                {
                    try
                    {
                        handler(message);
                    }
                    catch (Exception ex)
                    {
                        // Log error
                        Console.WriteLine($"Error processing event {eventName}: {ex.Message}");
                    }
                }
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
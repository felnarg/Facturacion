namespace Compras.Application.Abstractions;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, string routingKey, CancellationToken cancellationToken = default);
}

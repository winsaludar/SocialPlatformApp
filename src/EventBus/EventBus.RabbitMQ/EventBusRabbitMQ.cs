using EventBus.Core.Abstractions;
using EventBus.Core.Events;
using Microsoft.Extensions.Logging;

namespace EventBus.RabbitMQ;

public class EventBusRabbitMQ : IEventBus
{
    const string BROKER_NAME = "app_event_bus";

    private readonly IRabbitMQPersistentConnection _persistentConnection;
    private readonly ILogger<EventBusRabbitMQ> _logger;

    public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger)
    {
        _persistentConnection = persistentConnection;
        _logger = logger;
    }

    public void Publish(IntegrationEvent @event)
    {
        throw new NotImplementedException();
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
    {
        throw new NotImplementedException();
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
    {
        throw new NotImplementedException();
    }
}

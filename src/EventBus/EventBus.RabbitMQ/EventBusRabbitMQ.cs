using EventBus.Core.Abstractions;
using EventBus.Core.Events;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text.Json;

namespace EventBus.RabbitMQ;

public class EventBusRabbitMQ : IEventBus
{
    const string BROKER_NAME = "socialplatformapp.eventbus";

    private readonly IRabbitMQPersistentConnection _persistentConnection;
    private readonly ILogger<EventBusRabbitMQ> _logger;
    private readonly int _retryCount;

    public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, ILogger<EventBusRabbitMQ> logger, int retryCount = 5)
    {
        _persistentConnection = persistentConnection;
        _logger = logger;
        _retryCount = retryCount;
    }

    public void Publish(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
            });

        string eventName = @event.GetType().Name;
        _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

        using IModel channel = _persistentConnection.CreateModel();
        _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

        channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");
        var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions { WriteIndented = true });

        policy.Execute(() =>
        {
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent

            _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

            channel.BasicPublish(
                exchange: BROKER_NAME,
                routingKey: eventName,
                mandatory: true,
                basicProperties: properties,
                body: body);
        });
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

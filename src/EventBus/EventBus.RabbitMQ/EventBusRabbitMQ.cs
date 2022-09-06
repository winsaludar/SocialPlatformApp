using EventBus.Core;
using EventBus.Core.Abstractions;
using EventBus.Core.Events;
using EventBus.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static EventBus.Core.InMemoryEventBusSubscriptionsManager;

namespace EventBus.RabbitMQ;

public class EventBusRabbitMQ : IEventBus
{
    const string BROKER_NAME = "socialplatformapp.eventbus";

    private readonly IRabbitMQPersistentConnection _persistentConnection;
    private readonly ILogger<EventBusRabbitMQ> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IEventBusSubscriptionsManager _subscriptionsManager;
    private readonly int _retryCount;

    private IModel _consumerChannel;
    private string? _queueName;

    public EventBusRabbitMQ(
        IRabbitMQPersistentConnection persistentConnection,
        ILogger<EventBusRabbitMQ> logger,
        IServiceScopeFactory serviceScopeFactory,
        IEventBusSubscriptionsManager subscriptionsManager,
        string? queueName = null,
        int retryCount = 5)
    {
        _persistentConnection = persistentConnection;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _subscriptionsManager = subscriptionsManager;
        _queueName = queueName;
        _retryCount = retryCount;

        _consumerChannel = CreateConsumerChannel();
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

    public void SubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

        DoInternalSubscription(eventName);
        _subscriptionsManager.AddDynamicSubscription<TH>(eventName);
        StartBasicConsume();
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
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

    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        _logger.LogTrace("Creating RabbitMQ consumer channel");

        IModel channel = _persistentConnection.CreateModel();

        channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");
        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.CallbackException += (sender, ea) =>
        {
            _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

            _consumerChannel.Dispose();
            _consumerChannel = CreateConsumerChannel();
            StartBasicConsume();
        };

        return channel;
    }

    private void StartBasicConsume()
    {
        _logger.LogTrace("Starting RabbitMQ basic consume");

        if (_consumerChannel != null)
        {
            AsyncEventingBasicConsumer consumer = new(_consumerChannel);
            consumer.Received += Consumer_Received;

            _consumerChannel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        }
        else
        {
            _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
        }
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        string eventName = eventArgs.RoutingKey;
        string message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                throw new InvalidOperationException($"Fake exception requested: \"{message}\"");

            await ProcessEvent(eventName, message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
        }

        // Even on exception we take the message off the queue.
        // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX)
        // For more information see: https://www.rabbitmq.com/dlx.html
        _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        _logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

        if (!_subscriptionsManager.HasSubscriptionsForEvent(eventName))
        {
            _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        IEnumerable<SubscriptionInfo> subscriptions = _subscriptionsManager.GetHandlersForEvent(eventName);
        foreach (var subscription in subscriptions)
        {
            if (subscription.IsDynamic)
            {
                if (scope.ServiceProvider.GetService(subscription.HandlerType) is not IDynamicIntegrationEventHandler handler)
                    continue;

                using dynamic eventData = JsonDocument.Parse(message);
                await Task.Yield();
                await handler.Handle(eventData);
            }
            else
            {
                var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                if (handler == null)
                    continue;

                Type? eventType = _subscriptionsManager.GetEventTypeByName(eventName);
                if (eventType == null)
                    continue;

                var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                if (integrationEvent == null)
                    continue;

                Type concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                MethodInfo? handleMethod = concreteType.GetMethod("Handle");
                if (handleMethod == null)
                    continue;

                if (handleMethod.Invoke(handler, new object[] { integrationEvent }) is not Task handleMethodTask)
                    continue;

                await Task.Yield();
                await handleMethodTask;
            }
        }
    }

    private void DoInternalSubscription(string eventName)
    {
        bool containsKey = _subscriptionsManager.HasSubscriptionsForEvent(eventName);
        if (containsKey)
            return;

        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        _consumerChannel.QueueBind(queue: _queueName, exchange: BROKER_NAME, routingKey: eventName);
    }
}

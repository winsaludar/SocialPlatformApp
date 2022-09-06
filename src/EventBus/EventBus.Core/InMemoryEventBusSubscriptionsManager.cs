using EventBus.Core.Abstractions;
using EventBus.Core.Events;

namespace EventBus.Core;

public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
    private readonly List<Type> _eventTypes;

    public InMemoryEventBusSubscriptionsManager()
    {
        _handlers = new Dictionary<string, List<SubscriptionInfo>>();
        _eventTypes = new List<Type>();
    }

    public bool IsEmpty => _handlers is { Count: 0 };
    public event EventHandler<string>? OnEventRemoved;

    public void AddDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        DoAddSubscription(typeof(TH), eventName, isDynamic: true);
    }

    public void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        string eventName = GetEventKey<T>();

        DoAddSubscription(typeof(TH), eventName, isDynamic: false);

        if (!_eventTypes.Contains(typeof(T)))
            _eventTypes.Add(typeof(T));
    }

    public void RemoveSubscription<T, TH>()
        where TH : IIntegrationEventHandler<T>
        where T : IntegrationEvent
    {
        SubscriptionInfo? handlerToRemove = FindSubscriptionToRemove<T, TH>();
        string eventName = GetEventKey<T>();

        if (handlerToRemove != null)
            DoRemoveHandler(eventName, handlerToRemove);
    }

    public void RemoveDynamicSubscription<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        SubscriptionInfo? handlerToRemove = FindDynamicSubscriptionToRemove<TH>(eventName);

        if (handlerToRemove != null)
            DoRemoveHandler(eventName, handlerToRemove);
    }

    public bool HasSubscriptionsForEvent<T>()
        where T : IntegrationEvent
    {
        string key = GetEventKey<T>();
        return HasSubscriptionsForEvent(key);
    }

    public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public Type? GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(x => x.Name == eventName);

    public void Clear() => _handlers.Clear();

    public string GetEventKey<T>() => typeof(T).Name;

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>()
        where T : IntegrationEvent
    {
        string key = GetEventKey<T>();
        return GetHandlersForEvent(key);
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

    private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
    {
        if (!HasSubscriptionsForEvent(eventName))
            _handlers.Add(eventName, new List<SubscriptionInfo>());

        if (_handlers[eventName].Any(x => x.HandlerType == handlerType))
            throw new ArgumentException($"Handler Type '{handlerType.Name}' is already registered for '{eventName}'", nameof(handlerType));

        if (isDynamic)
            _handlers[eventName].Add(SubscriptionInfo.Dynamic(handlerType));
        else
            _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
    }

    private SubscriptionInfo? FindSubscriptionToRemove<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        string eventName = GetEventKey<T>();
        return DoFindSubscriptionToRemove(eventName, typeof(TH));
    }

    private SubscriptionInfo? FindDynamicSubscriptionToRemove<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        return DoFindSubscriptionToRemove(eventName, typeof(TH));
    }

    private SubscriptionInfo? DoFindSubscriptionToRemove(string eventName, Type handlerType)
    {
        if (!HasSubscriptionsForEvent(eventName))
            return null;

        return _handlers[eventName].SingleOrDefault(x => x.HandlerType == handlerType);
    }

    private void DoRemoveHandler(string eventName, SubscriptionInfo subToRemove)
    {
        _handlers[eventName].Remove(subToRemove);
        if (_handlers[eventName].Any())
            return;

        _handlers.Remove(eventName);
        Type? eventType = _eventTypes.SingleOrDefault(x => x.Name == eventName);
        if (eventType == null)
            return;

        _eventTypes.Remove(eventType);

        RaiseOnEventRemoved(eventName);
    }

    private void RaiseOnEventRemoved(string eventName)
    {
        var handler = OnEventRemoved;
        handler?.Invoke(this, eventName);
    }
}

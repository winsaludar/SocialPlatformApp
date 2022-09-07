using EventBus.Core.Abstractions;
using Space.IntegrationEvents.Events;

namespace Space.IntegrationEvents.EventHandlers;

public class UserRegisteredSuccessfulIntegrationEventHandler : IIntegrationEventHandler<UserRegisteredSuccessfulIntegrationEvent>
{
    public async Task Handle(UserRegisteredSuccessfulIntegrationEvent @event)
    {
        throw new NotImplementedException();
    }
}

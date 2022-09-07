using EventBus.Core.Abstractions;
using Space.Common.IntegrationEvents.Events;

namespace Space.Common.IntegrationEvents.EventHandlers;

public class UserRegisteredSuccessfulIntegrationEventHandler : IIntegrationEventHandler<UserRegisteredSuccessfulIntegrationEvent>
{
    public async Task Handle(UserRegisteredSuccessfulIntegrationEvent @event)
    {
        throw new NotImplementedException();
    }
}

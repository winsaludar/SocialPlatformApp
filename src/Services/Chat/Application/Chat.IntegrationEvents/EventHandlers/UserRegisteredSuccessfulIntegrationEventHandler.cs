using Chat.Domain.Repositories;
using Chat.IntegrationEvents.Events;
using EventBus.Core.Abstractions;

namespace Chat.IntegrationEvents.EventHandlers;

public class UserRegisteredSuccessfulIntegrationEventHandler : IIntegrationEventHandler<UserRegisteredSuccessfulIntegrationEvent>
{
    private readonly IRepositoryManager _reposityManager;

    public UserRegisteredSuccessfulIntegrationEventHandler(IRepositoryManager repositoryManager) => _reposityManager = repositoryManager;

    public async Task Handle(UserRegisteredSuccessfulIntegrationEvent @event)
    {
    }
}


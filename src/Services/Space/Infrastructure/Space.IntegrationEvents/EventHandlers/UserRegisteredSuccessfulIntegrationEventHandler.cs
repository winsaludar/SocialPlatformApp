using EventBus.Core.Abstractions;
using Space.Domain.Entities;
using Space.Domain.Repositories;
using Space.IntegrationEvents.Events;

namespace Space.IntegrationEvents.EventHandlers;

public class UserRegisteredSuccessfulIntegrationEventHandler : IIntegrationEventHandler<UserRegisteredSuccessfulIntegrationEvent>
{
    private readonly IRepositoryManager _reposityManager;

    public UserRegisteredSuccessfulIntegrationEventHandler(IRepositoryManager repositoryManager) => _reposityManager = repositoryManager;

    public async Task Handle(UserRegisteredSuccessfulIntegrationEvent @event)
    {
        await _reposityManager.UnitOfWork.BeginTransactionAsync();

        Soul newSoul = new()
        {
            Name = @event.Email,
            Email = @event.Email,
            CreatedBy = @event.Email,
            CreatedDateUtc = DateTime.UtcNow
        };

        await _reposityManager.SoulRepository.CreateAsync(newSoul);
        await _reposityManager.UnitOfWork.CommitAsync();
    }
}

using Mapster;
using Space.Contracts;
using Space.Domain.Entities;
using Space.Domain.Repositories;
using Space.Services.Abstraction;

namespace Space.Services;

public class SoulService : ISoulService
{
    private readonly IRepositoryManager _repositoryManager;

    public SoulService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task CreateSpaceAsync(SpaceDto dto)
    {
        Soul soul = new(_repositoryManager) { Email = dto.Creator };
        var space = dto.Adapt<Domain.Entities.Space>();

        await soul.CreateSpaceAsync(space);
    }

    public async Task JoinSpaceAsync(Guid spaceId, string email)
    {
        Soul soul = new(_repositoryManager) { Email = email };
        await soul.JoinSpaceAsync(spaceId);
    }

    public async Task LeaveSpaceAsync(Guid spaceId, string email)
    {
        Soul soul = new(_repositoryManager) { Email = email };
        await soul.LeaveSpaceAsync(spaceId);
    }
}

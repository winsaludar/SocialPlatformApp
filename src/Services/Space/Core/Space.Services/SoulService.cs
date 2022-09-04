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

    public async Task<IEnumerable<TopicDto>> GetAllTopicsAsync(Guid soulId)
    {
        var soul = await _repositoryManager.SoulRepository.GetByIdAsync(soulId, false, true);
        if (soul == null)
            return new List<TopicDto>();

        var result = soul.Topics.Adapt<List<TopicDto>>();
        foreach (var item in result)
        {
            item.AuthorEmail = soul.Email;
            item.AuthorUsername = soul.Name;
        }

        return result;
    }

    public async Task<IEnumerable<SpaceDto>> GetAllModeratedSpacesAsync(string email)
    {
        var soul = await _repositoryManager.SoulRepository.GetByEmailAsync(email, false, false, true);
        if (soul == null)
            return new List<SpaceDto>();

        return soul.SpacesAsModerator.Adapt<List<SpaceDto>>();
    }
}

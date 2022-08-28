using Mapster;
using Space.Contracts;
using Space.Domain.Helpers;
using Space.Domain.Repositories;
using Space.Services.Abstraction;

namespace Space.Services;

public class SpaceService : ISpaceService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IHelperManager _helperManager;

    public SpaceService(IRepositoryManager repositoryManager, IHelperManager helperManager)
    {
        _repositoryManager = repositoryManager;
        _helperManager = helperManager;
    }

    public async Task<IEnumerable<SpaceDto>> GetAllAsync()
    {
        var spaces = await _repositoryManager.SpaceRepository.GetAllAsync();
        if (spaces == null)
            return new List<SpaceDto>();

        return spaces.Adapt<List<SpaceDto>>();
    }

    public async Task KickSoulAsync(Guid spaceId, string email)
    {
        Domain.Entities.Space space = new() { Id = spaceId };
        await space.KickSoulAsync(email, _repositoryManager);
    }

    public async Task CreateTopicAsync(TopicDto dto)
    {
        Domain.Entities.Space space = new() { Id = dto.SpaceId };
        await space.CreateTopicAsync(dto.AuthorEmail, dto.Title, dto.Content, _repositoryManager, _helperManager);
    }
}

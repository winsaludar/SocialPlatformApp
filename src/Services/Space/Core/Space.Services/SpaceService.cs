using Mapster;
using Space.Contracts;
using Space.Domain.Repositories;
using Space.Services.Abstraction;

namespace Space.Services;

public class SpaceService : ISpaceService
{
    private readonly IRepositoryManager _repositoryManager;

    public SpaceService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

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
}

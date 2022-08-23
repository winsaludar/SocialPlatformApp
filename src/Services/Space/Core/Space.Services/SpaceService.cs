using Space.Contracts;
using Space.Domain.Repositories;
using Space.Services.Abstraction;
using DomainEntities = Space.Domain.Entities;

namespace Space.Services;

internal sealed class SpaceService : ISpaceService
{
    private readonly IRepositoryManager _repositoryManager;

    public SpaceService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task CreateAsync(SpaceDto dto)
    {
        DomainEntities.Space newSpace = new()
        {
            Name = dto.Name,
            Creator = dto.Creator,
            ShortDescription = dto.ShortDescription,
            LongDescription = dto.LongDescription,
            Thumbnail = dto.Thumbnail,
            CreatedDateUtc = DateTime.UtcNow,
            CreatedBy = dto.Creator
        };

        await _repositoryManager.SpaceRepository.CreateAsync(newSpace);
    }
}

using Space.Contracts;
using Space.Domain.Exceptions;
using Space.Domain.Repositories;
using Space.Services.Abstraction;
using DomainEntities = Space.Domain.Entities;

namespace Space.Services;

public class SpaceService : ISpaceService
{
    private readonly IRepositoryManager _repositoryManager;

    public SpaceService(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public async Task CreateAsync(SpaceDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name))
            throw new InvalidSpaceNameException(dto.Name);

        if (string.IsNullOrEmpty(dto.Creator))
            throw new InvalidSpaceCreatorException(dto.Creator);

        var existingSpace = await _repositoryManager.SpaceRepository.GetByNameAsync(dto.Name);
        if (existingSpace != null)
            throw new SpaceNameAlreadyExistException(dto.Name);

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

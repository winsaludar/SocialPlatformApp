﻿using Mapster;
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

    public async Task<IEnumerable<SpaceDto>> GetAllAsync()
    {
        var spaces = await _repositoryManager.SpaceRepository.GetAllAsync();
        if (spaces == null)
            return new List<SpaceDto>();

        return spaces.Adapt<List<SpaceDto>>();
    }

    public async Task CreateAsync(SpaceDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name))
            throw new InvalidSpaceNameException(dto.Name);

        if (string.IsNullOrEmpty(dto.Creator))
            throw new InvalidSpaceCreatorException(dto.Creator);

        var existingSpace = await _repositoryManager.SpaceRepository.GetByNameAsync(dto.Name);
        if (existingSpace != null)
            throw new SpaceNameAlreadyExistException(dto.Name);

        var newSpace = dto.Adapt<DomainEntities.Space>();
        newSpace.CreatedDateUtc = DateTime.UtcNow;
        newSpace.CreatedBy = dto.Creator;

        await _repositoryManager.SpaceRepository.CreateAsync(newSpace);
    }
}

﻿using Mapster;
using Space.Contracts;
using Space.Domain.Entities;
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
        Domain.Entities.Space space = new(_repositoryManager) { Id = spaceId };
        await space.KickSoulAsync(email);
    }

    public async Task<IEnumerable<TopicDto>> GetAllTopicsAsync(Guid spaceId)
    {
        Domain.Entities.Space space = new(_repositoryManager) { Id = spaceId };
        List<TopicDto> result = space.Topics.Adapt<List<TopicDto>>();
        foreach (var item in result)
        {
            Soul? author = await _repositoryManager.SoulRepository.GetByIdAsync(item.SoulId);
            if (author == null)
                continue;

            item.AuthorEmail = author.Email;
            item.AuthorUsername = author.Name;
        }

        return result;
    }

    public async Task CreateTopicAsync(TopicDto dto)
    {
        Domain.Entities.Space space = new(_repositoryManager, _helperManager) { Id = dto.SpaceId };
        await space.CreateTopicAsync(dto.AuthorEmail, dto.Title, dto.Content);
    }
}

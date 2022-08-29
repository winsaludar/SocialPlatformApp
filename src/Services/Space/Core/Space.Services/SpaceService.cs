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

    public async Task<IEnumerable<SoulDto>> GetAllSoulsAsync(Guid spaceId)
    {
        var space = await _repositoryManager.SpaceRepository.GetByIdAsync(spaceId, true, false);
        if (space == null)
            return new List<SoulDto>();

        List<SoulDto> result = space.Souls.Select(x => new SoulDto
        {
            Id = x.Id,
            Username = x.Name,
            Email = x.Email
        }).ToList();

        return result;
    }

    public async Task<IEnumerable<TopicDto>> GetAllTopicsAsync(Guid spaceId)
    {
        var space = await _repositoryManager.SpaceRepository.GetByIdAsync(spaceId, false, true);
        if (space == null)
            return new List<TopicDto>();

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

    public async Task KickSoulAsync(Guid spaceId, string email)
    {
        Domain.Entities.Space space = new(_repositoryManager) { Id = spaceId };
        await space.KickSoulAsync(email);
    }

    public async Task CreateTopicAsync(TopicDto dto)
    {
        Domain.Entities.Space space = new(_repositoryManager, _helperManager) { Id = dto.SpaceId };
        await space.CreateTopicAsync(dto.AuthorEmail, dto.Title, dto.Content);
    }

    public async Task UpdateTopicAsync(TopicDto dto)
    {
        Domain.Entities.Space space = new(_repositoryManager, _helperManager) { Id = dto.SpaceId };
        await space.UpdateTopicAsync(dto.Id, dto.AuthorEmail, dto.Title, dto.Content);
    }
}

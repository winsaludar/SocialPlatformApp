﻿using Space.Domain.Exceptions;
using Space.Domain.Helpers;
using Space.Domain.Repositories;

namespace Space.Domain.Entities;

public class MemberSoul : Soul
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IHelperManager _helperManager;

    public MemberSoul(string email, Guid spaceId, IRepositoryManager repositoryManager, IHelperManager helperManager)
    {
        Email = email;
        SpaceId = spaceId;
        _repositoryManager = repositoryManager;
        _helperManager = helperManager;
    }

    public Guid SpaceId { get; init; }

    public async Task<bool> IsMemberAsync()
    {
        return await _repositoryManager.SoulRepository.IsMemberOfSpaceAsync(Id, SpaceId);
    }

    public async Task CreateTopicAsync(string title, string content)
    {
        if (string.IsNullOrEmpty(Email))
            throw new InvalidSoulException(Email);

        await _repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure space id is valid
        Space? targetSpace = await _repositoryManager.SpaceRepository.GetByIdAsync(SpaceId);
        if (targetSpace == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(Id);
        }

        // Make sure author email is valid
        Soul? existingSoul = await _repositoryManager.SoulRepository.GetByEmailAsync(Email);
        if (existingSoul == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(Email);
        }

        // Make sure soul is a member
        bool isMember = await _repositoryManager.SoulRepository.IsMemberOfSpaceAsync(existingSoul.Id, SpaceId);
        if (!isMember)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulNotMemberException(Email, targetSpace.Name);
        }

        Topic newTopic = new(_helperManager)
        {
            Title = title,
            Content = content,
            SpaceId = targetSpace.Id,
            SoulId = existingSoul.Id,
            CreatedBy = existingSoul.Email,
            CreatedDateUtc = DateTime.UtcNow
        };

        await _repositoryManager.SpaceRepository.CreateTopicAsync(newTopic);
        await _repositoryManager.UnitOfWork.CommitAsync();
    }

    public async Task UpdateTopicAsync(Guid topicId, string modifiedBy, string updatedTitle, string updatedContent)
    {

    }

    public async Task DeleteTopicAsync(Guid topicId, string deletedBy)
    {

    }
}

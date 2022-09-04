﻿using Space.Domain.Exceptions;
using Space.Domain.Helpers;
using Space.Domain.Repositories;

namespace Space.Domain.Entities;

public class Space : BaseEntity
{
    private readonly IRepositoryManager? _repositoryManager;
    private readonly IHelperManager? _helperManager;

    public Space() { }

    public Space(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public Space(IRepositoryManager repositoryManager, IHelperManager helperManager)
    {
        _repositoryManager = repositoryManager;
        _helperManager = helperManager;
    }

    public string Creator { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string LongDescription { get; set; } = default!;
    public string? Thumbnail { get; set; }
    public IList<Soul> Members { get; set; } = new List<Soul>();
    public IList<Topic> Topics { get; set; } = new List<Topic>();
    public IList<Soul> Moderators { get; set; } = new List<Soul>();

    public async Task KickMemberAsync(string kickedByEmail, string memberEmail)
    {
        if (_repositoryManager == null)
            throw new NullReferenceException("IRepositoryManager is null");

        if (string.IsNullOrEmpty(kickedByEmail))
            throw new InvalidSoulException(kickedByEmail);

        if (string.IsNullOrEmpty(memberEmail))
            throw new InvalidSoulException(memberEmail);

        await _repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure space id is valid
        Space? targetSpace = await _repositoryManager.SpaceRepository.GetByIdAsync(Id);
        if (targetSpace == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(Id);
        }

        // Make sure kickedBy email is valid
        Soul? kickedBySoul = await _repositoryManager.SoulRepository.GetByEmailAsync(kickedByEmail);
        if (kickedBySoul == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(kickedByEmail);
        }

        // Make sure kickedBy email is a moderator
        bool isModerator = await _repositoryManager.SoulRepository.IsModeratorOfSpaceAsync(kickedBySoul.Id, targetSpace.Id);
        if (!isModerator)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new UnauthorizedAccessException($"'{kickedByEmail}' is not authorize to kick a member");
        }

        // Make sure member email is valid
        Soul? memberSoul = await _repositoryManager.SoulRepository.GetByEmailAsync(memberEmail);
        if (memberSoul == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(memberEmail);
        }

        // Make sure member email is a member
        bool isMember = await _repositoryManager.SoulRepository.IsMemberOfSpaceAsync(memberSoul.Id, Id);
        if (!isMember)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulNotMemberException(memberEmail, targetSpace.Name);
        }

        // Remove soul
        await _repositoryManager.SoulRepository.DeleteSpaceMemberAsync(memberSoul.Id, Id);
        await _repositoryManager.UnitOfWork.CommitAsync();
    }

    public async Task CreateTopicAsync(string authorEmail, string title, string content)
    {
        if (_repositoryManager == null)
            throw new NullReferenceException("IRepositoryManager is null");

        if (_helperManager == null)
            throw new NullReferenceException("IHelperManager is null");

        if (string.IsNullOrEmpty(authorEmail))
            throw new InvalidSoulException(authorEmail);

        await _repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure space id is valid
        Space? targetSpace = await _repositoryManager.SpaceRepository.GetByIdAsync(Id);
        if (targetSpace == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(Id);
        }

        // Make sure author email is valid
        Soul? existingSoul = await _repositoryManager.SoulRepository.GetByEmailAsync(authorEmail);
        if (existingSoul == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(authorEmail);
        }

        // Make sure soul is a member
        bool isMember = await _repositoryManager.SoulRepository.IsMemberOfSpaceAsync(existingSoul.Id, Id);
        if (!isMember)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulNotMemberException(authorEmail, targetSpace.Name);
        }

        Topic newTopic = new(_helperManager)
        {
            Title = title,
            Content = content,
            SpaceId = Id,
            SoulId = existingSoul.Id,
            CreatedBy = existingSoul.Email,
            CreatedDateUtc = DateTime.UtcNow
        };

        await _repositoryManager.SpaceRepository.CreateTopicAsync(newTopic);
        await _repositoryManager.UnitOfWork.CommitAsync();
    }

    public async Task UpdateTopicAsync(Guid topicId, string modifiedBy, string updatedTitle, string updatedContent)
    {
        if (_repositoryManager == null)
            throw new NullReferenceException("IRepositoryManager is null");

        if (_helperManager == null)
            throw new NullReferenceException("IHelperManager is null");

        if (string.IsNullOrEmpty(modifiedBy))
            throw new InvalidSoulException(modifiedBy);

        await _repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure topic id is valid
        Topic? existingTopic = await _repositoryManager.SpaceRepository.GetTopicByIdAsync(topicId);
        if (existingTopic == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidTopicIdException(topicId);
        }

        // Make sure space id is valid
        Space? targetSpace = await _repositoryManager.SpaceRepository.GetByIdAsync(Id);
        if (targetSpace == null || existingTopic.SpaceId != targetSpace.Id)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(Id);
        }

        // Make sure the modifier exist
        Soul? existingSoul = await _repositoryManager.SoulRepository.GetByEmailAsync(modifiedBy);
        if (existingSoul == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(modifiedBy);
        }

        // Only the author and the moderators of the space can edit the topic
        bool isModerator = await _repositoryManager.SoulRepository.IsModeratorOfSpaceAsync(existingSoul.Id, Id);
        if (existingTopic.SoulId != existingSoul.Id && !isModerator)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new UnauthorizedAccessException($"'{modifiedBy}' is not authorize to edit topic '{existingTopic.Id}'");
        }

        // Make sure soul is still a member
        bool isMember = await _repositoryManager.SoulRepository.IsMemberOfSpaceAsync(existingSoul.Id, Id);
        if (!isMember && !isModerator)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulNotMemberException(modifiedBy, targetSpace.Name);
        }

        existingTopic.Title = updatedTitle;
        existingTopic.Content = updatedContent;
        existingTopic.LastModifiedBy = modifiedBy;
        existingTopic.LastModifiedDateUtc = DateTime.UtcNow;

        await _repositoryManager.SpaceRepository.UpdateTopicAsync(existingTopic);
        await _repositoryManager.UnitOfWork.CommitAsync();
    }

    public async Task DeleteTopicAsync(Guid topicId, string deletedBy)
    {
        if (_repositoryManager == null)
            throw new NullReferenceException("IRepositoryManager is null");

        if (string.IsNullOrEmpty(deletedBy))
            throw new InvalidSoulException(deletedBy);

        await _repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure topic id is valid
        Topic? existingTopic = await _repositoryManager.SpaceRepository.GetTopicByIdAsync(topicId);
        if (existingTopic == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidTopicIdException(topicId);
        }

        // Make sure space id is valid
        Space? targetSpace = await _repositoryManager.SpaceRepository.GetByIdAsync(Id);
        if (targetSpace == null || existingTopic.SpaceId != targetSpace.Id)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(Id);
        }

        // Make sure the deleter exist
        Soul? existingSoul = await _repositoryManager.SoulRepository.GetByEmailAsync(deletedBy);
        if (existingSoul == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(deletedBy);
        }

        // Only the author and the moderators of the space can delete the topic
        bool isModerator = await _repositoryManager.SoulRepository.IsModeratorOfSpaceAsync(existingSoul.Id, Id);
        if (existingTopic.SoulId != existingSoul.Id && !isModerator)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new UnauthorizedAccessException($"'{deletedBy}' is not authorize to delete topic '{existingTopic.Id}'");
        }

        await _repositoryManager.SpaceRepository.DeleteTopicAsync(existingTopic);
        await _repositoryManager.UnitOfWork.CommitAsync();
    }
}

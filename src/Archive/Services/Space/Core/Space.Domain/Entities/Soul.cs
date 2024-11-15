﻿using Space.Domain.Exceptions;
using Space.Domain.Repositories;

namespace Space.Domain.Entities;

public class Soul : BaseEntity
{
    private readonly IRepositoryManager? _repositoryManager;

    public Soul() { }

    public Soul(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public IList<Space> SpacesAsMember { get; set; } = new List<Space>();
    public IList<Space> SpacesAsModerator { get; set; } = new List<Space>();
    public IList<Topic> Topics { get; set; } = new List<Topic>();
    public IList<Topic> TopicVotes { get; set; } = new List<Topic>();
    public IList<Comment> Comments { get; set; } = new List<Comment>();

    public async Task CreateSpaceAsync(Space newSpace)
    {
        if (_repositoryManager == null)
            throw new NullReferenceException("IRepositoryManager is null");

        if (string.IsNullOrEmpty(newSpace.Name))
            throw new InvalidSpaceNameException(newSpace.Name);

        if (string.IsNullOrEmpty(Email))
            throw new InvalidSoulException(Email);

        await _repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Create/Get soul info
        Soul? existingSoul = await _repositoryManager.SoulRepository.GetByEmailAsync(Email, true, true);
        if (existingSoul == null)
        {
            existingSoul = new()
            {
                Name = Email,
                Email = Email,
                CreatedBy = Email,
                CreatedDateUtc = DateTime.UtcNow
            };
            await _repositoryManager.SoulRepository.CreateAsync(existingSoul);
        }

        // Make sure space name does not exist yet
        var existingSpace = await _repositoryManager.SpaceRepository.GetByNameAsync(newSpace.Name);
        if (existingSpace != null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new SpaceNameAlreadyExistException(newSpace.Name);
        }

        newSpace.CreatedDateUtc = DateTime.UtcNow;
        newSpace.CreatedBy = Email;
        newSpace.Moderators.Add(existingSoul);
        newSpace.Members.Add(existingSoul);
        await _repositoryManager.SpaceRepository.CreateAsync(newSpace);

        await _repositoryManager.UnitOfWork.CommitAsync();
    }

    public async Task JoinSpaceAsync(Guid spaceId)
    {
        if (_repositoryManager == null)
            throw new NullReferenceException("IRepositoryManager is null");

        if (string.IsNullOrEmpty(Email))
            throw new InvalidSoulException(Email);

        await _repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure space is valid
        Space? targetSpace = await _repositoryManager.SpaceRepository.GetByIdAsync(spaceId);
        if (targetSpace == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(spaceId);
        }

        // Create/Get soul info
        Soul? existingSoul = await _repositoryManager.SoulRepository.GetByEmailAsync(Email);
        if (existingSoul == null)
        {
            existingSoul = new()
            {
                Name = Email,
                Email = Email,
                CreatedBy = Email,
                CreatedDateUtc = DateTime.UtcNow
            };
            await _repositoryManager.SoulRepository.CreateAsync(existingSoul);
        }

        // Make sure soul is not a member yet of the target space
        bool isMemberAlready = await _repositoryManager.SoulRepository.IsMemberOfSpaceAsync(existingSoul.Id, spaceId);
        if (isMemberAlready)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulMemberAlreadyException(existingSoul.Email, targetSpace.Name);
        }

        targetSpace.Members.Add(existingSoul);
        await _repositoryManager.SpaceRepository.UpdateAsync(targetSpace);
        await _repositoryManager.UnitOfWork.CommitAsync();
    }

    public async Task LeaveSpaceAsync(Guid spaceId)
    {
        if (_repositoryManager == null)
            throw new NullReferenceException("IRepositoryManager is null");

        if (string.IsNullOrEmpty(Email))
            throw new InvalidSoulException(Email);

        await _repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure space is valid
        Space? targetSpace = await _repositoryManager.SpaceRepository.GetByIdAsync(spaceId);
        if (targetSpace == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(spaceId);
        }

        // Make sure soul is valid
        Soul? existingSoul = await _repositoryManager.SoulRepository.GetByEmailAsync(Email);
        if (existingSoul == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(Email);
        }

        // Make sure soul is a member of the target space
        bool isMember = await _repositoryManager.SoulRepository.IsMemberOfSpaceAsync(existingSoul.Id, spaceId);
        if (!isMember)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulNotMemberException(Email, targetSpace.Name);
        }

        await _repositoryManager.SoulRepository.DeleteSpaceMemberAsync(existingSoul.Id, spaceId);
        await _repositoryManager.UnitOfWork.CommitAsync();
    }
}

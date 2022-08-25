﻿using Space.Domain.Exceptions;
using Space.Domain.Repositories;

namespace Space.Domain.Entities;

public class Soul : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public IList<Space> Spaces { get; set; } = new List<Space>();

    public async Task CreateSpaceAsync(Space newSpace, IRepositoryManager repositoryManager)
    {
        if (string.IsNullOrEmpty(newSpace.Name))
            throw new InvalidSpaceNameException(newSpace.Name);

        if (string.IsNullOrEmpty(Email))
            throw new InvalidSoulException(Email);

        await repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Create/Get soul info
        Soul? existingSoul = await repositoryManager.SoulRepository.GetByEmailAsync(Email);
        if (existingSoul == null)
        {
            Name = Email;
            CreatedBy = Email;
            CreatedDateUtc = DateTime.UtcNow;
            await repositoryManager.SoulRepository.CreateAsync(this);
        }
        else
        {
            Id = existingSoul.Id;
            Name = existingSoul.Name;
            CreatedBy = existingSoul.CreatedBy;
            CreatedDateUtc = existingSoul.CreatedDateUtc;
            LastModifiedBy = existingSoul.LastModifiedBy;
            LastModifiedDateUtc = existingSoul.LastModifiedDateUtc;
        }

        // Make sure space name does not exist yet
        var existingSpace = await repositoryManager.SpaceRepository.GetByNameAsync(newSpace.Name);
        if (existingSpace != null)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new SpaceNameAlreadyExistException(newSpace.Name);
        }

        newSpace.CreatedDateUtc = DateTime.UtcNow;
        newSpace.CreatedBy = Email;
        newSpace.Souls.Add(this);

        await repositoryManager.SpaceRepository.CreateAsync(newSpace);
        await repositoryManager.UnitOfWork.CommitAsync();
    }

    public async Task JoinSpaceAsync(Guid spaceId, IRepositoryManager repositoryManager)
    {
        if (string.IsNullOrEmpty(Email))
            throw new InvalidSoulException(Email);

        await repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure space is valid
        Space? targetSpace = await repositoryManager.SpaceRepository.GetByIdAsync(spaceId);
        if (targetSpace == null)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(spaceId);
        }

        // Create/Get soul info
        Soul? existingSoul = await repositoryManager.SoulRepository.GetByEmailAsync(Email);
        if (existingSoul == null)
        {
            Name = Email;
            CreatedBy = Email;
            CreatedDateUtc = DateTime.UtcNow;
            await repositoryManager.SoulRepository.CreateAsync(this);
        }
        else
        {
            Id = existingSoul.Id;
            Name = existingSoul.Name;
            CreatedBy = existingSoul.CreatedBy;
            CreatedDateUtc = existingSoul.CreatedDateUtc;
            LastModifiedBy = existingSoul.LastModifiedBy;
            LastModifiedDateUtc = existingSoul.LastModifiedDateUtc;
        }

        // Make sure soul is not a member yet of the target space
        bool isMemberAlready = await repositoryManager.SoulRepository.IsMemberOfSpaceAsync(Id, spaceId);
        if (isMemberAlready)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulMemberAlreadyException(Email, targetSpace.Name);
        }

        targetSpace.Souls.Add(this);
        await repositoryManager.SpaceRepository.UpdateAsync(targetSpace);
        await repositoryManager.UnitOfWork.CommitAsync();
    }

    public async Task LeaveSpaceAsync(Guid spaceId, IRepositoryManager repositoryManager)
    {
        if (string.IsNullOrEmpty(Email))
            throw new InvalidSoulException(Email);

        await repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure space is valid
        Space? targetSpace = await repositoryManager.SpaceRepository.GetByIdAsync(spaceId);
        if (targetSpace == null)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(spaceId);
        }

        // Make sure soul is valid
        Soul? existingSoul = await repositoryManager.SoulRepository.GetByEmailAsync(Email);
        if (existingSoul == null)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(Email);
        }

        // Make sure soul is a member of the target space
        bool isMember = await repositoryManager.SoulRepository.IsMemberOfSpaceAsync(existingSoul.Id, spaceId);
        if (!isMember)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulNotMemberException(Email, targetSpace.Name);
        }

        await repositoryManager.SoulRepository.DeleteSoulSpaceAsync(existingSoul.Id, spaceId);
        await repositoryManager.UnitOfWork.CommitAsync();
    }
}

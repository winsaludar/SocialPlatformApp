using Space.Domain.Exceptions;
using Space.Domain.Repositories;

namespace Space.Domain.Entities;

public class Soul : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public IList<Space> Spaces { get; set; } = new List<Space>();

    public async Task JoinSpaceAsync(Guid spaceId, IRepositoryManager repositoryManager)
    {
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

        // Make sure space is valid
        Space? targetSpace = await repositoryManager.SpaceRepository.GetByIdAsync(spaceId);
        if (targetSpace == null)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(spaceId);
        }

        // Make sure soul is not a member yet of the target space
        bool isMemberAlready = await repositoryManager.SoulRepository.IsMemberOfSpace(Id, spaceId);
        if (isMemberAlready)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulMemberAlreadyException(Email, targetSpace.Name);
        }

        targetSpace.Souls.Add(this);
        await repositoryManager.SpaceRepository.UpdateAsync(targetSpace);
        await repositoryManager.UnitOfWork.CommitAsync();
    }
}

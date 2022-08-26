using Space.Domain.Exceptions;
using Space.Domain.Repositories;

namespace Space.Domain.Entities;

public class Space : BaseEntity
{
    public string Creator { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string LongDescription { get; set; } = default!;
    public string? Thumbnail { get; set; }
    public IList<Soul> Souls { get; set; } = new List<Soul>();

    public async Task KickSoulAsync(string email, IRepositoryManager repositoryManager)
    {
        if (string.IsNullOrEmpty(email))
            throw new InvalidSoulException(email);

        await repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure space id is valid
        Space? targetSpace = await repositoryManager.SpaceRepository.GetByIdAsync(Id);
        if (targetSpace == null)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(Id);
        }

        // Make sure soul id is valid
        Soul? existingSoul = await repositoryManager.SoulRepository.GetByEmailAsync(email);
        if (existingSoul == null)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(email);
        }

        // Make sure soul is a member
        bool isMember = await repositoryManager.SoulRepository.IsMemberOfSpaceAsync(existingSoul.Id, Id);
        if (!isMember)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulNotMemberException(email, targetSpace.Name);
        }

        // Remove soul
        await repositoryManager.SoulRepository.DeleteSoulSpaceAsync(existingSoul.Id, Id);
        await repositoryManager.UnitOfWork.CommitAsync();
    }
}

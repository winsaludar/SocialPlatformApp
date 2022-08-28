using Space.Domain.Exceptions;
using Space.Domain.Helpers;
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
    public IList<Topic> Topics { get; set; } = new List<Topic>();

    public async Task KickSoulAsync(string email, IRepositoryManager repositoryManager)
    {
        if (string.IsNullOrEmpty(email))
            throw new InvalidSoulException(email);

        // TODO: Only the admins of this space can kick a member

        await repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure space id is valid
        Space? targetSpace = await repositoryManager.SpaceRepository.GetByIdAsync(Id);
        if (targetSpace == null)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(Id);
        }

        // Make sure soul email is valid
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

    public async Task CreateTopicAsync(string authorEmail, string title, string content, IRepositoryManager repositoryManager, IHelperManager helperManager)
    {
        if (string.IsNullOrEmpty(authorEmail))
            throw new InvalidSoulException(authorEmail);

        await repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure space id is valid
        Space? targetSpace = await repositoryManager.SpaceRepository.GetByIdAsync(Id);
        if (targetSpace == null)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSpaceIdException(Id);
        }

        // Make sure author email is valid
        Soul? existingSoul = await repositoryManager.SoulRepository.GetByEmailAsync(authorEmail);
        if (existingSoul == null)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(authorEmail);
        }

        // Make sure soul is a member
        bool isMember = await repositoryManager.SoulRepository.IsMemberOfSpaceAsync(existingSoul.Id, Id);
        if (!isMember)
        {
            await repositoryManager.UnitOfWork.RollbackAsync();
            throw new SoulNotMemberException(authorEmail, targetSpace.Name);
        }

        Topic newTopic = new(helperManager)
        {
            Title = title,
            Content = content,
            SpaceId = Id,
            SoulId = existingSoul.Id,
            CreatedBy = existingSoul.Email,
            CreatedDateUtc = DateTime.UtcNow
        };

        await repositoryManager.SpaceRepository.CreateTopicAsync(newTopic);
        await repositoryManager.UnitOfWork.CommitAsync();
    }
}

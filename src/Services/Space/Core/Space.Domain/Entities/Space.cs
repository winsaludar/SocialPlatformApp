using Space.Domain.Exceptions;
using Space.Domain.Helpers;
using Space.Domain.Repositories;

namespace Space.Domain.Entities;

public class Space : BaseEntity
{
    private readonly IRepositoryManager? _repositoryManager;
    private readonly IHelperManager? _helperManager;
    private string _name = default!;

    public Space() { }

    public Space(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public Space(IRepositoryManager repositoryManager, IHelperManager helperManager)
    {
        _repositoryManager = repositoryManager;
        _helperManager = helperManager;
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            if (_helperManager != null)
                Slug = _helperManager.SlugHelper.CreateSlug(value);
        }
    }
    public string Creator { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string LongDescription { get; set; } = default!;
    public string Slug { get; private set; } = default!;
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
}

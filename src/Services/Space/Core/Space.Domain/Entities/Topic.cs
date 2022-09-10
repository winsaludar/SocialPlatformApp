using Space.Domain.Exceptions;
using Space.Domain.Helpers;
using Space.Domain.Repositories;

namespace Space.Domain.Entities;

public class Topic : BaseEntity
{
    private readonly IRepositoryManager? _repositoryManager;
    private readonly IHelperManager? _helperManager;
    private string _title = default!;

    public Topic() { }

    public Topic(IRepositoryManager repositoryManager, IHelperManager helperManager)
    {
        _repositoryManager = repositoryManager;
        _helperManager = helperManager;
    }

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            if (_helperManager != null)
                Slug = _helperManager.SlugHelper.CreateSlug(value);
        }
    }
    public string Content { get; set; } = default!;
    public string Slug { get; private set; } = default!;
    public Guid SpaceId { get; set; }
    public Guid? SoulId { get; set; }
    public Space Space { get; set; } = default!;
    public Soul? Soul { get; set; } = default!;
    public IList<Soul> SoulVoters { get; set; } = new List<Soul>();

    public async Task UpvoteAsync(string voterEmail)
    {
        await ProcessVoteAsync(voterEmail, 1, 0);
    }

    public async Task DownvoteAsync(string voterEmail)
    {
        await ProcessVoteAsync(voterEmail, 0, 1);
    }

    private async Task ProcessVoteAsync(string voterEmail, int upvote, int downvote)
    {
        if (_repositoryManager == null)
            throw new NullReferenceException("IRepositoryManager is null");

        await _repositoryManager.UnitOfWork.BeginTransactionAsync();

        // Make sure topic id is valid
        Topic? targetTopic = await _repositoryManager.SpaceRepository.GetTopicByIdAsync(Id);
        if (targetTopic == null || targetTopic.SpaceId != SpaceId)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidTopicIdException(Id);
        }

        // Make sure voter email exist
        Soul? existingSoul = await _repositoryManager.SoulRepository.GetByEmailAsync(voterEmail);
        if (existingSoul == null)
        {
            await _repositoryManager.UnitOfWork.RollbackAsync();
            throw new InvalidSoulException(voterEmail);
        }

        // Insert/Update vote
        SoulTopicVote? vote = await _repositoryManager.SoulRepository.GetTopicVoteAsync(existingSoul.Id, targetTopic.Id);
        if (vote == null)
        {
            vote = new()
            {
                SoulId = existingSoul.Id,
                TopicId = targetTopic.Id,
                Upvote = upvote,
                Downvote = downvote
            };
            await _repositoryManager.SoulRepository.CreateTopicVoteAsync(vote);
        }
        else
        {
            vote.Downvote = downvote;
            vote.Upvote = upvote;
            await _repositoryManager.SoulRepository.UpdateTopicVoteAsync(vote);
        }

        await _repositoryManager.UnitOfWork.CommitAsync();
    }
}

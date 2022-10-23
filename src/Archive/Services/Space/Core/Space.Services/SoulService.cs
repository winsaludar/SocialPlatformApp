using Mapster;
using Space.Contracts;
using Space.Domain.Entities;
using Space.Domain.Helpers;
using Space.Domain.Repositories;
using Space.Services.Abstraction;

namespace Space.Services;

public class SoulService : ISoulService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IHelperManager _helperManager;

    public SoulService(IRepositoryManager repositoryManager, IHelperManager helperManager)
    {
        _repositoryManager = repositoryManager;
        _helperManager = helperManager;
    }

    public async Task CreateSpaceAsync(SpaceDto dto)
    {
        Soul soul = new(_repositoryManager) { Email = dto.Creator };
        Domain.Entities.Space newSpace = new(_helperManager)
        {
            Name = dto.Name,
            Creator = dto.Creator,
            ShortDescription = dto.ShortDescription,
            LongDescription = dto.LongDescription,
            Thumbnail = dto.Thumbnail
        };

        await soul.CreateSpaceAsync(newSpace);
    }

    public async Task JoinSpaceAsync(Guid spaceId, string email)
    {
        Soul soul = new(_repositoryManager) { Email = email };
        await soul.JoinSpaceAsync(spaceId);
    }

    public async Task LeaveSpaceAsync(Guid spaceId, string email)
    {
        Soul soul = new(_repositoryManager) { Email = email };
        await soul.LeaveSpaceAsync(spaceId);
    }

    public async Task<IEnumerable<TopicDto>> GetAllTopicsByIdAsync(Guid soulId)
    {
        var soul = await _repositoryManager.SoulRepository.GetByIdAsync(soulId, includeTopics: true);
        if (soul == null)
            return new List<TopicDto>();

        var result = soul.Topics.Adapt<List<TopicDto>>();
        foreach (var item in result)
        {
            item.AuthorEmail = soul.Email;
            item.AuthorUsername = soul.Name;

            (item.Upvotes, item.Downvotes) = await _repositoryManager.SpaceRepository.GetTopicVotesAsync(item.Id);
        }

        return result;
    }

    public async Task<IEnumerable<TopicDto>> GetAllTopicsByEmailAsync(string email)
    {
        var soul = await _repositoryManager.SoulRepository.GetByEmailAsync(email, includeTopics: true);
        if (soul == null)
            return new List<TopicDto>();

        var result = soul.Topics.Adapt<List<TopicDto>>();
        foreach (var item in result)
        {
            item.AuthorEmail = soul.Email;
            item.AuthorUsername = soul.Name;

            (item.Upvotes, item.Downvotes) = await _repositoryManager.SpaceRepository.GetTopicVotesAsync(item.Id);
        }

        return result;
    }

    public async Task<IEnumerable<CommentDto>> GetAllCommentsByEmailAsync(string email)
    {
        var soul = await _repositoryManager.SoulRepository.GetByEmailAsync(email, includeComments: true);
        if (soul == null)
            return new List<CommentDto>();

        var result = soul.Comments.Adapt<List<CommentDto>>();
        foreach (var item in result)
        {
            item.AuthorEmail = soul.Email;
            item.AuthorUsername = soul.Name;
        }

        return result;
    }

    public async Task<IEnumerable<SpaceDto>> GetAllModeratedSpacesByEmailAsync(string email)
    {
        var soul = await _repositoryManager.SoulRepository.GetByEmailAsync(email, includeModeratedSpaces: true);
        if (soul == null)
            return new List<SpaceDto>();

        return soul.SpacesAsModerator.Adapt<List<SpaceDto>>();
    }
}

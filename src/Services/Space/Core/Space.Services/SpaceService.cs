using Mapster;
using Space.Contracts;
using Space.Domain.Entities;
using Space.Domain.Helpers;
using Space.Domain.Repositories;
using Space.Services.Abstraction;

namespace Space.Services;

public class SpaceService : ISpaceService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IHelperManager _helperManager;

    public SpaceService(IRepositoryManager repositoryManager, IHelperManager helperManager)
    {
        _repositoryManager = repositoryManager;
        _helperManager = helperManager;
    }

    public async Task<IEnumerable<SpaceDto>> GetAllAsync()
    {
        var spaces = await _repositoryManager.SpaceRepository.GetAllAsync();
        if (spaces == null)
            return new List<SpaceDto>();

        return spaces.Adapt<List<SpaceDto>>();
    }

    public async Task<SpaceDto?> GetByIdAsync(Guid spaceId)
    {
        var space = await _repositoryManager.SpaceRepository.GetByIdAsync(spaceId);
        if (space == null)
            return null;

        return space.Adapt<SpaceDto>();
    }

    public async Task<SpaceDto?> GetBySlugAsync(string slug)
    {
        if (string.IsNullOrEmpty(slug))
            return null;

        var space = await _repositoryManager.SpaceRepository.GetBySlugAsync(slug);
        if (space == null)
            return null;

        return space.Adapt<SpaceDto>();
    }

    public async Task<IEnumerable<SoulDto>> GetAllModeratorsAsync(Guid spaceId)
    {
        var space = await _repositoryManager.SpaceRepository.GetByIdAsync(spaceId, false, false, true);
        if (space == null)
            return new List<SoulDto>();

        List<SoulDto> result = space.Moderators.Select(x => new SoulDto
        {
            Id = x.Id,
            Username = x.Name,
            Email = x.Email
        }).ToList();

        return result;
    }

    public async Task<IEnumerable<SoulDto>> GetAllMembersAsync(Guid spaceId)
    {
        var space = await _repositoryManager.SpaceRepository.GetByIdAsync(spaceId, true, false);
        if (space == null)
            return new List<SoulDto>();

        List<SoulDto> result = space.Members.Select(x => new SoulDto
        {
            Id = x.Id,
            Username = x.Name,
            Email = x.Email
        }).ToList();

        return result;
    }

    public async Task<IEnumerable<TopicDto>> GetAllTopicsAsync(Guid spaceId)
    {
        var space = await _repositoryManager.SpaceRepository.GetByIdAsync(spaceId, false, true);
        if (space == null)
            return new List<TopicDto>();

        List<TopicDto> result = space.Topics.Adapt<List<TopicDto>>();
        foreach (var item in result)
        {
            Soul? author = await _repositoryManager.SoulRepository.GetByIdAsync(item.SoulId);
            if (author == null)
                continue;

            item.AuthorEmail = author.Email;
            item.AuthorUsername = author.Name;

            (item.Upvotes, item.Downvotes) = await _repositoryManager.SpaceRepository.GetTopicVotesAsync(item.Id);
        }

        return result;
    }

    public async Task<TopicDto?> GetTopicByIdAsync(Guid spaceId, Guid topicId)
    {
        var space = await _repositoryManager.SpaceRepository.GetByIdAsync(spaceId);
        if (space == null)
            return null;

        var topic = await _repositoryManager.SpaceRepository.GetTopicByIdAsync(topicId);
        if (topic == null || topic.SpaceId != space.Id)
            return null;

        var result = topic.Adapt<TopicDto>();

        Soul? author = await _repositoryManager.SoulRepository.GetByIdAsync(result.SoulId);
        if (author != null)
        {
            result.AuthorEmail = author.Email;
            result.AuthorUsername = author.Name;
        }

        (result.Upvotes, result.Downvotes) = await _repositoryManager.SpaceRepository.GetTopicVotesAsync(result.Id);

        return result;
    }

    public async Task<TopicDto?> GetTopicBySlugAsync(string spaceSlug, string topicSlug)
    {
        var space = await _repositoryManager.SpaceRepository.GetBySlugAsync(spaceSlug);
        if (space == null)
            return null;

        var topic = await _repositoryManager.SpaceRepository.GetTopicBySlugAsync(topicSlug);
        if (topic == null || topic.SpaceId != space.Id)
            return null;

        var result = topic.Adapt<TopicDto>();

        Soul? author = await _repositoryManager.SoulRepository.GetByIdAsync(result.SoulId);
        if (author != null)
        {
            result.AuthorEmail = author.Email;
            result.AuthorUsername = author.Name;
        }

        (result.Upvotes, result.Downvotes) = await _repositoryManager.SpaceRepository.GetTopicVotesAsync(result.Id);

        return result;
    }

    public async Task CreateTopicAsync(TopicDto dto)
    {
        MemberSoul member = new(dto.AuthorEmail, dto.SpaceId, _repositoryManager, _helperManager);
        await member.CreateTopicAsync(dto.Title, dto.Content);
    }

    public async Task UpdateTopicAsync(TopicDto dto)
    {
        MemberSoul member = new(dto.AuthorEmail, dto.SpaceId, _repositoryManager, _helperManager);
        await member.UpdateTopicAsync(dto.Id, dto.Title, dto.Content);
    }

    public async Task DeleteTopicAsync(TopicDto dto)
    {
        MemberSoul member = new(dto.AuthorEmail, dto.SpaceId, _repositoryManager, _helperManager);
        await member.DeleteTopicAsync(dto.Id);
    }

    public async Task UpvoteTopicAsync(Guid spaceId, Guid topicId, string voterEmail)
    {
        Topic topic = new(_repositoryManager, _helperManager) { Id = topicId, SpaceId = spaceId };
        await topic.UpvoteAsync(voterEmail);
    }

    public async Task DownvoteTopicAsync(Guid spaceId, Guid topicId, string voterEmail)
    {
        Topic topic = new(_repositoryManager, _helperManager) { Id = topicId, SpaceId = spaceId };
        await topic.DownvoteAsync(voterEmail);
    }

    public async Task UnvoteTopicAsync(Guid spaceId, Guid topicId, string voterEmail)
    {
        Topic topic = new(_repositoryManager, _helperManager) { Id = topicId, SpaceId = spaceId };
        await topic.UnvoteAsync(voterEmail);
    }

    public async Task<IEnumerable<CommentDto>> GetAllCommentsAsync(Guid spaceId, Guid topicId)
    {
        var space = await _repositoryManager.SpaceRepository.GetByIdAsync(spaceId);
        if (space == null)
            return new List<CommentDto>();

        var topic = await _repositoryManager.SpaceRepository.GetTopicByIdAsync(topicId, true);
        if (topic == null || topic.SpaceId != space.Id)
            return new List<CommentDto>();

        var comments = topic.Comments.Adapt<List<CommentDto>>();
        foreach (var comment in comments)
        {
            Soul? author = await _repositoryManager.SoulRepository.GetByIdAsync(comment.SoulId);
            if (author != null)
            {
                comment.AuthorEmail = author.Email;
                comment.AuthorUsername = author.Name;
            }
        }

        return comments;
    }

    public async Task CreateCommentAsync(CommentDto dto)
    {
        Topic topic = new(_repositoryManager, _helperManager) { Id = dto.TopicId, SpaceId = dto.SpaceId };
        Comment newComment = new() { Content = dto.Content };
        await topic.AddCommentAsync(dto.AuthorEmail, newComment);
    }

    public async Task KickMemberAsync(Guid spaceId, string kickedByEmail, string memberEmail)
    {
        ModeratorSoul moderator = new(kickedByEmail, spaceId, _repositoryManager, _helperManager);
        await moderator.KickMemberAsync(memberEmail);
    }
}

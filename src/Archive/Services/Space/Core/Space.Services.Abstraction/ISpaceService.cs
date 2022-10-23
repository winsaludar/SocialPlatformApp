using Space.Contracts;

namespace Space.Services.Abstraction;

public interface ISpaceService
{
    Task<IEnumerable<SpaceDto>> GetAllAsync();
    Task<SpaceDto?> GetByIdAsync(Guid spaceId);
    Task<SpaceDto?> GetBySlugAsync(string slug);

    Task<IEnumerable<SoulDto>> GetAllModeratorsAsync(Guid spaceId);
    Task<IEnumerable<SoulDto>> GetAllMembersAsync(Guid spaceId);
    Task<IEnumerable<TopicDto>> GetAllTopicsAsync(Guid spaceId);

    Task<TopicDto?> GetTopicByIdAsync(Guid spaceId, Guid topicId);
    Task<TopicDto?> GetTopicBySlugAsync(string spaceSlug, string topicSlug);
    Task CreateTopicAsync(TopicDto dto);
    Task UpdateTopicAsync(TopicDto dto);
    Task DeleteTopicAsync(TopicDto dto);
    Task UpvoteTopicAsync(Guid spaceId, Guid topicId, string voterEmail);
    Task DownvoteTopicAsync(Guid spaceId, Guid topicId, string voterEmail);
    Task UnvoteTopicAsync(Guid spaceId, Guid topicId, string voterEmail);

    Task<IEnumerable<CommentDto>> GetAllCommentsAsync(Guid spaceId, Guid topicId);
    Task CreateCommentAsync(CommentDto dto);
    Task UpdateCommentAsync(CommentDto dto);
    Task DeleteCommentAsync(CommentDto dto);

    Task KickMemberAsync(Guid spaceId, string kickedByEmail, string memberEmail);
}

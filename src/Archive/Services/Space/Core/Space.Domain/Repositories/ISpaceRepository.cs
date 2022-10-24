using Space.Domain.Entities;

namespace Space.Domain.Repositories;

public interface ISpaceRepository
{
    Task<IEnumerable<Entities.Space>> GetAllAsync(bool includeMembers = false, bool includeTopics = false, bool includeModerators = false);
    Task<Entities.Space?> GetByNameAsync(string name, bool includeMembers = false, bool includeTopics = false, bool includeModerators = false);
    Task<Entities.Space?> GetByIdAsync(Guid id, bool includeMembers = false, bool includeTopics = false, bool includeModerators = false);
    Task<Entities.Space?> GetBySlugAsync(string slug, bool includeMembers = false, bool includeTopics = false, bool includeModerators = false);
    Task CreateAsync(Entities.Space newSpace);
    Task UpdateAsync(Entities.Space space);

    Task<Topic?> GetTopicByIdAsync(Guid topicId, bool includeComments = false);
    Task<Topic?> GetTopicBySlugAsync(string topicSlug, bool includeComments = false);
    Task CreateTopicAsync(Topic newTopic);
    Task UpdateTopicAsync(Topic topic);
    Task DeleteTopicAsync(Topic topic);
    Task<(int upvotes, int downvotes)> GetTopicVotesAsync(Guid topicId);

    Task<Comment?> GetCommentByIdAsync(Guid commentId);
    Task CreateCommentAsync(Comment newComment);
    Task UpdateCommentAsync(Comment comment);
    Task DeleteCommentAsync(Comment comment);
}

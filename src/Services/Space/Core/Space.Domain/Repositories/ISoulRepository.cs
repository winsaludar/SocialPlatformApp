using Space.Domain.Entities;

namespace Space.Domain.Repositories;

public interface ISoulRepository
{
    Task<Soul?> GetByEmailAsync(
        string email,
        bool includeMemberSpaces = false,
        bool includeTopics = false,
        bool includeModeratedSpaces = false);
    Task<Soul?> GetByIdAsync(
        Guid id,
        bool includeMemberSpaces = false,
        bool includeTopics = false,
        bool includeModeratedSpaces = false);
    Task CreateAsync(Soul newSoul);
    Task UpdateAsync(Soul soul);
    Task<bool> IsMemberOfSpaceAsync(Guid soulId, Guid spaceId);
    Task<bool> IsModeratorOfSpaceAsync(Guid soulId, Guid spaceId);
    Task DeleteSpaceMemberAsync(Guid soulId, Guid spaceId);

    Task<SoulTopicVote?> GetTopicVoteAsync(Guid soulId, Guid topicId);
    Task CreateTopicVoteAsync(SoulTopicVote newVote);
    Task UpdateTopicVoteAsync(SoulTopicVote vote);
}

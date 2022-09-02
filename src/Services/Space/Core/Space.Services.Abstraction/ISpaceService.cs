using Space.Contracts;

namespace Space.Services.Abstraction;

public interface ISpaceService
{
    Task<IEnumerable<SpaceDto>> GetAllAsync();
    Task KickMemberAsync(Guid spaceId, string email);
    Task<IEnumerable<SoulDto>> GetAllMembersAsync(Guid spaceId);
    Task<IEnumerable<TopicDto>> GetAllTopicsAsync(Guid spaceId);
    Task CreateTopicAsync(TopicDto dto);
    Task UpdateTopicAsync(TopicDto dto);
    Task DeleteTopicAsync(TopicDto dto);
}
